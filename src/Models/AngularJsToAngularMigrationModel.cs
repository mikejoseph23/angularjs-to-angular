using System;
using System.Collections.Generic;

namespace Models
{
    public class AngularJsToAngularMigrationModel
    {
        public ICollection<MigrationType> MigrationTypes { get; set; }
    }

    public class MigrationType
    {
        public string Extension { get; set; }

        public ICollection<SearchReplaceOperation> Replacements { get; set; }
    }

    public class SearchReplaceOperation
    {
        public string Old { get; set; }

        public string New { get; set; }

        public string AddToConstructor { get; set; }
    }
}
