using System.Data;
using Microsoft.AspNetCore.Http;

namespace UseCases;

public interface IExcelFileReader
{
    DataTable ToDataTable(IFormFile file);
    DataSet ToDataSet(IFormFile file);
}