namespace RVTR.Lodging.DataContext
{
    /// <summary>
    /// This class is used for storing URI request parameters
    /// </summary>
    public class ReviewSearchFilterModel : SearchFilter
    {
        private double _ratingAtLeast;
        /// <summary>
        /// Minimum star rating.
        /// </summary>
        public double RatingAtLeast
        {
            get {
                if (_ratingAtLeast < 0) return 0;
                if (_ratingAtLeast > 10) return 10;
                return _ratingAtLeast;
            }
            set { _ratingAtLeast = value; }
        }

        private int? _lodgingId;
        /// <summary>
        /// Get reviews for this Lodging ID.
        /// </summary>
        public int? LodgingId
        {
            get { return _lodgingId; }
            set { _lodgingId = value; }
        }

        private int? _accountId;
        /// <summary>
        /// Get reviews for this Account ID.
        /// </summary>
        /// <value></value>
        public int? AccountId
        {
            get { return _accountId; }
            set { _accountId = value; }
        }
    }
}
