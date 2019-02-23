using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IAnalyser
    {
        Task<AnalysisResult> AnalyzeAsync(DataContainer dataContainer);
    }
}
