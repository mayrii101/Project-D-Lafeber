using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using ProjectD.Models;
using ProjectD.Services;

[ApiController]
[Route("api/[controller]")]
public class XmlImportController : ControllerBase
{
    private readonly XmlToSqlImporter _importer;
    private readonly ILogger<XmlImportController> _logger;

    public XmlImportController(XmlToSqlImporter importer, ILogger<XmlImportController> logger)
    {
        _importer = importer;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadXmlFiles([FromForm] XmlFilesUploadDto filesDto)
    {
        if (filesDto.XmlFile1 == null || filesDto.XmlFile2 == null)
        {
            return BadRequest("Both XML files must be provided.");
        }

        try
        {
            string xmlContent1;
            string xmlContent2;

            using (var reader1 = new StreamReader(filesDto.XmlFile1.OpenReadStream()))
            {
                xmlContent1 = await reader1.ReadToEndAsync();
            }

            using (var reader2 = new StreamReader(filesDto.XmlFile2.OpenReadStream()))
            {
                xmlContent2 = await reader2.ReadToEndAsync();
            }

            await _importer.ImportFromTwoXmlStringsAsync(xmlContent1, xmlContent2);

            return Ok(new { message = "Files imported successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import XML files");
            return StatusCode(500, "Internal server error while importing XML files.");
        }
    }
}

// DTO for model binding uploaded files
public class XmlFilesUploadDto
{
    [FromForm(Name = "xmlFile1")]
    public IFormFile XmlFile1 { get; set; }

    [FromForm(Name = "xmlFile2")]
    public IFormFile XmlFile2 { get; set; }
}