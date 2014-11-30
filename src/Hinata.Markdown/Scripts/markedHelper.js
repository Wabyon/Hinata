var markedHelper = (function (marked) {
    "use strict";

    var exports = {},
        defaultOptions = {
            gfm: true,
            tables: true,
            breaks: true,
            pedantic: false,
            sanitize: false,
            smartLists: true,
            silent: false,
            highlight: function (code) {
                return hljs.highlightAuto(code).value;
            },
            langPrefix: '',
            smartypants: false,
            headerPrefix: '',
            renderer: new marked.Renderer(),
            xhtml: false
        };

    // add the class *table* so they'll be styled correctly
    defaultOptions.renderer.table = function (header, body) {
        return '<table class="table table-bordered">\n'
            + '<thead>\n'
            + header
            + '</thead>\n'
            + '<tbody>\n'
            + body
            + '</tbody>\n'
            + '</table>\n';
    };

    function extend(destination, source) {
        var propertyName;

        destination = destination || {};

        for (propertyName in source) {
            if (source.hasOwnProperty(propertyName)) {
                destination[propertyName] = source[propertyName];
            }
        }

        return destination;
    }

    exports.compile = function (markdown, options) {
        var compilationOptions;

        options = options || {};
        compilationOptions = extend(extend({}, defaultOptions), options);

        return marked(markdown, compilationOptions);
    };

    return exports;
}(marked));
