namespace Chocolatey.Language.Server.Models
{
    /// <summary>
    /// Base class for holding common values.
    /// </summary>
    public abstract class MetaValue
    {
        private int? _textStart;
        private int? _textEnd;

        /// <summary>
        /// Gets or sets the index where the element for this value starts.
        /// </summary>
        public int ElementStart { get; set; }

        /// <summary>
        /// Gets or sets the index where the element for this value ends.
        /// </summary>
        public int ElementEnd { get; set; }

        /// <summary>
        /// Gets or sets the index where the actual text starts.
        /// </summary>
        public int TextStart
        {
            get
            {
                return _textStart ?? ElementStart;
            }
            set
            {
                _textStart = value;
            }
        }

        /// <summary>
        /// Gets or sets the index where the actual text ends.
        /// </summary>
        public int TextEnd
        {
            get
            {
                return _textEnd ?? ElementEnd;
            }
            set
            {
                _textEnd = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element has changed from the last time it was parsed.
        /// </summary>
        /// <value><c>true</c> if this element has changed; otherwise, <c>false</c>.</value>
        public bool HasChanged { get; set; } = true; // Need to figure out a way to get changes

        /// <summary>
        /// Gets or sets a value indicating whether this element is missing from the nuspec xml.
        /// </summary>
        /// <value><c>true</c> if this element is missing; otherwise, <c>false</c>.</value>
        public bool IsMissing { get; set; }
    }

    /// <summary>
    /// A simple class for holding the value of an element, as well as the location of the element.
    /// This class cannot be inherited. Implements the <see cref="T:Chocolatey.Language.Server.Models.MetaValue"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value this class hold.</typeparam>
    public sealed class MetaValue<TValue> : MetaValue
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Chocolatey.Language.Server.Models.MetaValue`1"/> class with a default value and
        /// <see cref="MetaValue.IsMissing"/> set to <c>true</c>.
        /// </summary>
        public MetaValue()
            : this(default)
        {
            IsMissing = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Chocolatey.Language.Server.Models.MetaValue`1"/> class.
        /// </summary>
        /// <param name="value">The value for this element.</param>
        public MetaValue(TValue value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the value for this element.
        /// </summary>
        /// <value>The value of the element.</value>
        public TValue Value { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see
        /// cref="T:Chocolatey.Language.Server.Models.MetaValue`1"/> to <see cref="T:Chocolatey.Language.Server.Models.MetaValue`1.TValue"/>.
        /// </summary>
        /// <param name="metaValue">The meta value to do the implici conversion from.</param>
        /// <returns>The value of the conversion of the specified meta value class.</returns>
        public static implicit operator TValue(MetaValue<TValue> metaValue)
        {
            if (metaValue == null)
            {
                return default;
            }

            return metaValue.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
