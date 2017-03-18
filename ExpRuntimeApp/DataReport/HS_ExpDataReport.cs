using System;
using ExpRuntimeApp.Modules;

namespace ExpRuntimeApp.DataReport
{


    partial class HS_ExpDataReport
    {
        partial class HS_DataReportDataTable
        {
            public MdChannel RlFlowChannel { get; set; }
            public MdChannel YlFlowChannel { get; set; }
            public MdChannel YlInTempChannel { get; set; }
            public MdChannel ElFlowChannel { get; set; }
            public MdChannel ElInTempChannel { get; set; }
            public MdChannel ElOutTempChannel { get; set; }
            public MdChannel RlInPressChannel { get; set; }
            public MdChannel RlOutpressChannel { get; set; }
            public MdChannel RlInTempChannel { get; set; }
            public MdChannel RlOutTempChannel { get; set; }
            public MdChannel ElInPressChannel { get; set; }
            public MdChannel ElOutPressChannel { get; set; }
            public MdChannel RlPressDiffChannel { get; set; }
            public MdChannel ElPressDiffChannel { get; set; }
            public MdChannel HeatEmissEffecChannel { get; set; }

            /// <summary>
            /// 立即将当时数据添加进表。
            /// </summary>
            public HS_DataReportRow AddRowNow()
            {
                var nRow = NewHS_DataReportRow();
                nRow._dateTime = DateTime.Now;
                nRow._rlFlow = RlFlowChannel?.MeasureValue ?? 0;
                nRow._ylFlow = YlFlowChannel?.MeasureValue ?? 0;
                nRow._ylInTemp = YlInTempChannel?.MeasureValue ?? 0;
                nRow._elFlow = ElFlowChannel?.MeasureValue ?? 0;
                nRow._elInTemp = ElInTempChannel?.MeasureValue ?? 0;
                nRow._elOutTemp = ElOutTempChannel?.MeasureValue ?? 0;
                nRow._rlInPress = RlInPressChannel?.MeasureValue ?? 0;
                nRow._rlOutPress = RlOutpressChannel?.MeasureValue ?? 0;
                nRow._rlInTemp = RlInTempChannel?.MeasureValue ?? 0;
                nRow._rlOutTemp = RlOutTempChannel?.MeasureValue ?? 0;
                nRow._elInPress = ElInPressChannel?.MeasureValue ?? 0;
                nRow._elOutPress = ElOutPressChannel?.MeasureValue ?? 0;
                nRow._rlPressDiff = RlPressDiffChannel?.MeasureValue ?? 0;
                nRow._elPressDiff = ElPressDiffChannel?.MeasureValue ?? 0;
                nRow._heatEmissEffec = HeatEmissEffecChannel?.MeasureValue ?? 0;
                AddHS_DataReportRow(nRow);

                return nRow;
            }
        }
    }
}
