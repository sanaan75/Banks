﻿using UseCases.Interfaces;

namespace UseCases;

public class GetFileByte : IGetFileByte
{
    private IDb _db;

    public GetFileByte(IDb db)
    {
        _db = db;
    }

    public string Respond(int? id)
    {
        // var image = _unitOfWork.Files.GetById(id.Value);
        // string base64 = "data:image/jpg;base64," + Convert.ToBase64String(image.Data ?? Array.Empty<byte>(), 0,
        //     image.Data!.Length);
        // return base64;
        return "";
    }
}