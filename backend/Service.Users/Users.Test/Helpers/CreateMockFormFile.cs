using Microsoft.AspNetCore.Http;

public static class FormFileHelper
{
    public static IFormFile CreateMockFormFile(byte[] fileContents, string fileName = "test.jpg", string contentType = "image/jpeg")
    {
        var stream = new MemoryStream(fileContents);
        var formFile = new FormFile(stream, 0, fileContents.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
        return formFile;
    }
}
