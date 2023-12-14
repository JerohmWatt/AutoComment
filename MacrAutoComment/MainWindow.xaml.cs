using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;

namespace MacrAutoComment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Config config;
        string csharpFunctionPattern = @"\b\w+\s+\w+\s+\w+\(.+?\)";
        string methodSignature;
        List<String> parametersList;

        public MainWindow()
        {
            InitializeComponent();
            LoadConfig();
            ProcessClipboard();
        }

        private void ExtractMethodSignatureAndParameters(string methodCode)
        {
            // Entourer la méthode avec une classe pour créer une unité syntaxique complète
            string classWithMethodCode = $"class DummyClass {{ {methodCode} }}";

            // Utiliser CSharpSyntaxTree pour analyser le code
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classWithMethodCode);

            // Rechercher les déclarations de méthodes dans le code
            IEnumerable<MethodDeclarationSyntax> methods = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (MethodDeclarationSyntax method in methods)
            {
                // Extraire les paramètres de la méthode en tant que liste de chaînes
                parametersList = method.ParameterList.Parameters.Select(parameter => parameter.ToString()).ToList();

                // Extraire le nom de la méthode
                string methodName = method.Identifier.ValueText;

                // Construire la signature complète de la méthode
                methodSignature = $"{method.Modifiers} {method.ReturnType} {methodName}({string.Join(", ", parametersList)})";

                // Afficher la signature de méthode
                Console.WriteLine($"Signature de méthode : {methodSignature}");
            }
        }
    

        private void LoadConfig()
        {
            string configFilePath = "C:/temp/autocomment.txt";
                string configJson = File.ReadAllText(configFilePath);
                config = JsonConvert.DeserializeObject<Config>(configJson);
        }  
        
        private void ProcessClipboard()
        {
            string clipboardText = Clipboard.GetText();
            if (!string.IsNullOrEmpty(clipboardText))
            {
                ExtractMethodSignatureAndParameters(clipboardText);
                string formattedComment = GenerateComment();

                // Mettez à jour le presse-papiers avec le nouveau commentaire
                Clipboard.SetText(formattedComment);

            }
        }

        /* [private string GenerateComment()]
* DESCRIPTION
*
*@version 1.0 (VotreNom) DESCRIPTION 
*
* @fscodes FS0000
*/
        private string GenerateComment()
        {
            // Ajoutez un préfixe au commentaire pour distinguer les commentaires générés
            string result = $"/* [{methodSignature}]\n" +
                   "* DESCRIPTION\n" +
                   "*\n*";
            foreach(string param in parametersList)
            {
                result += $"@param {param} DESCRIPTION \n*";
            }
            if(parametersList.Count == 0)
            {
                result = RemoveTrailingNewLineAsterisks(result);
            }
            result += $"\n*@version {config.VERSION} ({config.AUTHOR}) DESCRIPTION \n*\n"+
                    $"* @fscodes FS0000\n" +
                   "*/";

            return result;
        }
        /* [static string RemoveTrailingNewLineAsterisks(string input)]
* DESCRIPTION
*
*@param string input DESCRIPTION 
*
*@version 1.0 (VotreNom) DESCRIPTION 
*
* @fscodes FS0000
*/
        static string RemoveTrailingNewLineAsterisks(string input)
        {
            // Trouver l'index du dernier retour à la ligne
            int lastNewLineIndex = input.LastIndexOf('\n');

            // Retirer "\n*" si présent à la fin de la chaîne
            if (lastNewLineIndex != -1 && input.EndsWith("*", StringComparison.Ordinal))
            {
                return input.Substring(0, lastNewLineIndex);
            }

            return input;
        }
    }
}

