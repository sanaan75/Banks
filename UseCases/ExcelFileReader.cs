using System.Data;
using ExcelDataReader;
using ExcelDataReader.Exceptions;
using Microsoft.AspNetCore.Http;

namespace UseCases;

public class ExcelFileReader : IExcelFileReader
{
    public DataTable ToDataTable(IFormFile file)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            using var reader = ExcelReaderFactory.CreateReader(memoryStream);
            Check.True(reader.FieldCount <= 50, () => "تعداد ستون مجاز نیست");
            var dataSetConfiguration = GetDataSetConfiguration(true);
            var dataSet = reader.AsDataSet(dataSetConfiguration);
            return dataSet.Tables.Cast<DataTable>().First();
        }
        catch (HeaderException e)
        {
            throw new AppException("امکان پردازش اطلاعات فایل اکسل وجود ندارد: " + e.Message);
        }
    }

    public DataSet ToDataSet(IFormFile file)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            using var reader = ExcelReaderFactory.CreateReader(memoryStream);
            Check.True(reader.FieldCount <= 50, () => "تعداد ستون مجاز نیست");
            var dataSetConfiguration = GetDataSetConfiguration(true);
            return reader.AsDataSet(dataSetConfiguration);
        }
        catch (HeaderException e)
        {
            throw new AppException("امکان پردازش اطلاعات فایل اکسل وجود ندارد: " + e.Message);
        }
    }

    private ExcelDataSetConfiguration GetDataSetConfiguration(bool useHeaderRow)
    {
        return new ExcelDataSetConfiguration
        {
            UseColumnDataType = true,
            ConfigureDataTable = tableReader => new ExcelDataTableConfiguration
            {
                EmptyColumnNamePrefix = "Column",
                UseHeaderRow = useHeaderRow,

                FilterRow = rowReader => true,
                FilterColumn = (rowReader, columnIndex) => true
            }
        };
    }
}