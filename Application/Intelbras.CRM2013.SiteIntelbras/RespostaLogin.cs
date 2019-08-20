using System.Collections.Generic;

namespace Intelbras.CRM2013.Application.SiteIntelbras
{
    public class Data {
        public string ckeditor_default {
            get;
            set;
        }
        public string ckeditor_show_toggle {
            get;
            set;
        }
        public string ckeditor_width {
            get;
            set;
        }
        public string ckeditor_lang {
            get;
            set;
        }
        public string ckeditor_auto_lang {
            get;
            set;
        }
    }

    public class Roles {
        public string __invalid_name__2 {
            get;
            set;
        }
        public string __invalid_name__3 {
            get;
            set;
        }
    }

    public class Name {
        public List<string> predicates {
            get;
            set;
        }
    }

    public class Homepage {
        public List<string> predicates {
            get;
            set;
        }
        public string type {
            get;
            set;
        }
    }

    public class RdfMapping {
        public List<string> rdftype {
            get;
            set;
        }
        public Name name {
            get;
            set;
        }
        public Homepage homepage {
            get;
            set;
        }
    }

    public class User {
        public string uid {
            get;
            set;
        }
        public string name {
            get;
            set;
        }
        public string mail {
            get;
            set;
        }
        public string theme {
            get;
            set;
        }
        public string signature {
            get;
            set;
        }
        public string signature_format {
            get;
            set;
        }
        public string created {
            get;
            set;
        }
        public string access {
            get;
            set;
        }
        public int login {
            get;
            set;
        }
        public string status {
            get;
            set;
        }
        public string timezone {
            get;
            set;
        }
        public string language {
            get;
            set;
        }
        public object picture {
            get;
            set;
        }
        public string init {
            get;
            set;
        }
        public Data data {
            get;
            set;
        }
        public string uuid {
            get;
            set;
        }
        public Roles roles {
            get;
            set;
        }
        public RdfMapping rdf_mapping {
            get;
            set;
        }
    }

    public class RespostaLogin {
        public string sessid {
            get;
            set;
        }
        public string session_name {
            get;
            set;
        }
        public User user {
            get;
            set;
        }
    }
}
