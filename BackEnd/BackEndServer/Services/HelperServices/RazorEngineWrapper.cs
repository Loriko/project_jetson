using System;
using System.IO;
using System.Threading.Tasks;
using RazorLight;
using System.Collections.Generic;

namespace BackEndServer.Services.HelperServices
{ 
    public class RazorEngineWrapper
    {
        //Add a dictionary for the template
        private static Dictionary<string, string> TEMPLATE_KEY_MAP = new Dictionary<string, string>();
        
        /// <summary>
        /// Generate an HTML document from the specified Razor template and model.
        /// </summary>
        /// <param name="rootpath">The path to the folder containing the Razor templates</param>
        /// <param name="templatename">The name of the Razor template (.cshtml)</param>
        /// <param name="model">The model containing the information to be supplied to the Razor template</param>
        /// <returns>Compiled</returns>
        /// <source>Adapted from https://medium.com/@DomBurf/templated-html-emails-using-razorengine-6f150bb5f3a6</source>
        public static string RunCompile(string rootpath, string templatename, object model)
        {
            string result = string.Empty;
            string templatekey;
            
            if (string.IsNullOrEmpty(rootpath) || string.IsNullOrEmpty(templatename) || model == null) return result;
 
            string templateFilePath = Path.Combine(rootpath, templatename);
 
            if (File.Exists(templateFilePath))
            {
                string template = File.ReadAllText(templateFilePath);
 
                //Instead of the if null or empty, make a lookup in the table for the template key
                if (TEMPLATE_KEY_MAP.ContainsKey(templateFilePath))
                {
                    templatekey = TEMPLATE_KEY_MAP[templateFilePath];
                }
                else
                {
                    templatekey = Guid.NewGuid().ToString();
                    TEMPLATE_KEY_MAP[templateFilePath] = templatekey;
                }

                var engine = new RazorLightEngineBuilder()
                    .UseMemoryCachingProvider()
                    .Build();

                Task<string> task = engine.CompileRenderAsync(templatekey, template, model);
                result = task.GetAwaiter().GetResult();
            }
            return result;
        }
    }
}