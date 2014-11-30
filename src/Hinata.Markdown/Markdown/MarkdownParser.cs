using System;
using Hinata.Internals;
using JavaScriptEngineSwitcher.Core;

namespace Hinata.Markdown
{
    public class MarkdownParser : IMarkdownParser, IDisposable
    {
        private readonly object _compilationSynchronizer = new object();

        private IJsEngine _jsEngine;
        private bool _initialized;
        private bool _disposed;

        public MarkdownParser(IJsEngine jsEngine)
        {
            if (jsEngine == null) throw new ArgumentNullException("jsEngine");
            _jsEngine = jsEngine;
        }

        private void Initialize()
        {
            if (_initialized) return;

            var type = GetType();

            _jsEngine.ExecuteResource("Hinata.Scripts.marked.js", type);
            _jsEngine.ExecuteResource("Hinata.Scripts.markedHelper.js", type);
            _jsEngine.ExecuteResource("Hinata.Scripts.highlight.pack.js", type);

            _initialized = true;
        }

        public string Transform(string markdown)
        {
            string result;

            lock (_compilationSynchronizer)
            {
                Initialize();

                _jsEngine.SetVariableValue("_markdownString", markdown);

                result = _jsEngine.Evaluate<string>("markedHelper.compile(_markdownString)");
            }

            return HtmlUtility.SanitizeHtml(result);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_jsEngine == null) return;
            _jsEngine.Dispose();
            
            _jsEngine = null;
        }
    }
}
