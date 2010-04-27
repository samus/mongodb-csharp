using System;

namespace MongoDB
{
    /// <summary>
    /// 
    /// </summary>
    public class CodeWScope : Code
    {
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public Document Scope {get;set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeWScope"/> class.
        /// </summary>
        public CodeWScope(){}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeWScope"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        public CodeWScope(String code):this(code, new Document()){}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeWScope"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="scope">The scope.</param>
        public CodeWScope(String code, Document scope){
            this.Value = code;
            this.Scope = scope;
        }
    }
}
