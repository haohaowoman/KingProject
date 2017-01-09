using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService
{
    public interface IExpAreaElementListen
    {
        /// <summary>
        /// Get all experiment areas.
        /// </summary>
        IReadOnlyList<ExperimentalArea> ExperimnetAreas { get; }


        /// <summary>
        /// Get all experiment points.
        /// </summary>
        List<ExperimentalPoint> AllExperimentPoints { get; }


        /// <summary>
        /// Look up a experiment area with label.
        /// </summary>
        /// <param name="label">Assign a experiment area label.</param>
        /// <returns>Return null if there is no this area as label.</returns>
        ExperimentalArea LookUpExpArea(string label);

        /// <summary>
        /// Invoke this event when experiment area's points have been changed.
        /// </summary>
        event NotifyCollectionChangedEventHandler ExperimentPointsChanged;
    }
}
