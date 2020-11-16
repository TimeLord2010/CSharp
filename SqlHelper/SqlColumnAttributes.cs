using SqlHelper.Attributes;

namespace SqlHelper {

    public class SqlColumnAttributes {

        public bool IsUnique { get; set; }
        public PrimaryKey PrimaryKey { get; set; }
        public Identity Identity { get; set; }
        public ForeignKey ForeignKey { get; set; }

    }

}