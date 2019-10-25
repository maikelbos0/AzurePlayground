using System;

namespace AzurePlayground.Database.ReferenceEntities {
    internal abstract class BaseReferenceEntity<TEnum> where TEnum : Enum {
        public TEnum Id { get; set; }
        public string Name { get; set; }
    }
}