using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SyntaxTree.VisualStudio.Unity.Bridge;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;

namespace CodeGeneration
{


    [InitializeOnLoad]
    public partial class CodeGenerator
    {
        static string[] tags => UnityEditorInternal.InternalEditorUtility.tags;
        static string[] layers => UnityEditorInternal.InternalEditorUtility.layers;

        public static bool AutoGenerate
        {
            get { return _autoGenerate; }
            set
            {
                //Debug.Log($"{value}, from {_autoGenerate}");
                if(value == _autoGenerate) return;
                _autoGenerate = value;
                Menu.SetChecked(menuName + "/" + nameof(AutoGenerateCode), value);
                EditorPrefs.SetBool("AutoUpdate", value);
            }
        }

        const string menuName = "Code Generation";
        static bool _autoGenerate = false;



        [MenuItem(menuName + "/"+nameof(GenerateCode))]
        public static void GenerateCode()
        {
            Generate(); 
        }

        [DidReloadScripts]
        static void Apply()
        {
            if (AutoGenerate) Generate();
        }

        [MenuItem(menuName + "/" + nameof(AutoGenerateCode))]
        public static void AutoGenerateCode()
        {
                AutoGenerate = !AutoGenerate;
        }


        static void Generate()
        {

            var generator = new CodeGenerator();
            var classDefintion = generator.TransformText();
            var outputPath = Path.Combine(Application.dataPath,"Scripts","Utils", "GeneratedCode.cs"); 
            try
            {
                if(File.Exists(outputPath+".bak")) File.Delete(outputPath + ".bak");
                if(File.Exists(outputPath)) File.Copy(outputPath, outputPath +".bak");
                // Save new class to assets folder.
                File.WriteAllText(outputPath, classDefintion); 
                // Refresh assets.
                //AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.Log("An error occurred while saving file: " + e);
            }
        }
        
        // necessary for XLinq to save the xml project file in utf8  
        class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        static CodeGenerator()
        {
            var types = Orb.AllOrbTypes;
            EditorApplication.playmodeStateChanged += () =>
            {
                Menu.SetChecked(menuName + "/" + nameof(AutoGenerateCode), AutoGenerate);

            };
            EditorApplication.delayCall += () =>
            {

                AutoGenerate = EditorPrefs.GetBool("AutoUpdate", true);
                Menu.SetChecked(menuName + "/" + nameof(AutoGenerateCode), AutoGenerate);

            };
            EditorApplication.update += () =>
            {

                Menu.SetChecked(menuName + "/" + nameof(AutoGenerateCode), AutoGenerate);
            };
            EditorApplication.RepaintProjectWindow();
            ProjectFilesGenerator.ProjectFileGeneration += OnProjectFileGeneration;

            var defaultOrbs = Resources.LoadAll("Defaults");
            foreach (var t in types.Where(t=>!t.IsAbstract))
            {
                if (defaultOrbs.Any(o => o.GetType() == t)) continue;
                var ass = ScriptableObject.CreateInstance(t);

                var outputPath = ($"Assets/OrbPrefabs/Resources/Defaults/Default{t.Name}.asset");
                
                AssetDatabase.CreateAsset(ass,outputPath );
            }
        }

        static string OnProjectFileGeneration(string name, string content)
        {
            // parse the document and make some changes  
            var document = XDocument.Parse(content);
            var project = document.Root;
            if (project == null) throw new Exception("There was a problem in an earlier hook.");
            var itemGroups = project.Elements().Where(e => e.Name.LocalName == "ItemGroup");
            var Nones = itemGroups.SelectMany(i => i.Elements().Where(e => e.Name.LocalName == "None"));
            var elements = Nones.Where(n => n.Elements().Any(e => e.Name.LocalName == "Generator"));

            var newItemGroup = new XElement(XName.Get("ItemGroup", project.Name.NamespaceName));
            foreach (var element in elements)
            {
                element.Remove();
                element.Name = XName.Get("Content", project.Name.NamespaceName);
                var xn = XName.Get("CustomToolNamepace", project.Name.NamespaceName);

                var ns = element.Element(xn) ?? new XElement(xn);
                ns.Value = nameof(CodeGeneration);
                newItemGroup.Add(element);
            }
            project.Add(newItemGroup);
            // save the changes using the Utf8StringWriter  
            var str = new Utf8StringWriter();
            document.Save(str);

            return str.ToString();
        }

        class AutoCodeGenerator : UnityEditor.AssetModificationProcessor
        {
            static string[] OnWillSaveAssets(string[] args)
            {
                Generate();
                return args;
            }
        }
    }


}