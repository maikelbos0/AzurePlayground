using System.Collections.Generic;
using AzurePlayground.Models;

namespace AzurePlayground.Services {
    public interface IDataGridViewService {
        IEnumerable<TViewModel> ApplyMetaData<TViewModel>(IEnumerable<TViewModel> query, ref DataGridViewMetaData metaData) where TViewModel : ViewModel;
    }
}