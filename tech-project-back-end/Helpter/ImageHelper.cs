using System.Text.RegularExpressions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace tech_project_back_end.Helpter;

public class ImageHelper
{
    public async static Task<ImageUploadResult> ImageUploadFunc(IFormFile file, Cloudinary cloudinary)
    {
        var uploadResult = new ImageUploadResult();
        
        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
            };
            uploadResult = await cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    public async static Task<bool> DeleteImage(string imageHref, Cloudinary cloudinary)
    {
        string pattern = @"([^/]+)(?=\.\w+$)";
        Match match = Regex.Match(imageHref, pattern);
        var success = await cloudinary.DestroyAsync(new DeletionParams(match.Value));
        return success.StatusCode == System.Net.HttpStatusCode.OK;
    }
    
}