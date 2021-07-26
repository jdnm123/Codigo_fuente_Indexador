using Microsoft.ML.Data;
using System.Collections.Generic;

namespace Microsoft.ML.Models.BERT.Input
{
    internal class BertFeature
    {
        //[VectorType(1)]
        //[ColumnName("unique_ids:0")]
        //public long[] UniqueIds { get; set; }

        [VectorType(1, 512)]
        [ColumnName("attention_mask")]
        public long[] InputMask { get; set; }

        [VectorType(1, 512)]
        [ColumnName("input.2")]
        public long[] token_type_ids { get; set; }

        [VectorType(1, 512)]
        [ColumnName("input.1")]
        public long[] InputIds { get; set; }
    }
}
