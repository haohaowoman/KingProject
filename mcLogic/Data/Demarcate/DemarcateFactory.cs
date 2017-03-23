using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Data;
namespace mcLogic.Demarcate
{
    public static class DemarcateFactory
    {
        public static LinerDemarcater LinerDemFromSamplePoints(SamplePointCollection sps)
        {
            if (sps == null)
            {
                throw new ArgumentNullException("输入参数不能为null。");
            }
            if (sps.Count < 2)
            {
                throw new ArgumentException("无效参数，输入样点集合样点数小于2。");
            }
            var ld = new LinerDemarcater();
            if (sps[0].X - sps[1].X == 0)
            {
                ld.Param1 = 0;
            }
            else
            {
                ld.Param1 = (sps[0].Y - sps[1].Y) / (sps[0].X - sps[1].X);
            }

            ld.Param0 = sps[0].Y - ld.Param1 * sps[0].X;

            return ld;
        }

        public static RangeDemarcater RangeDemFromSamplePoints(SamplePointCollection sps)
        {
            if (sps == null)
            {
                throw new ArgumentNullException("输入参数不能为null。");
            }
            if (sps.Count < 2)
            {
                throw new ArgumentException("无效参数，输入样点集合样点数小于2。");
            }
            var rd = new RangeDemarcater(new SafeRange(sps[0].X, sps[1].X), new SafeRange(sps[0].Y, sps[1].Y));
            return rd;
        }
    }
}
