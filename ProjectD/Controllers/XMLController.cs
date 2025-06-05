using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using ProjectD.Models;
using ProjectD.Services;
using Microsoft.AspNetCore.Http;

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
        _logger.LogInformation("Received request to upload XML files.");

        if (filesDto.XmlFile1 == null || filesDto.XmlFile2 == null)
        {
            _logger.LogWarning("Upload request missing one or both XML files.");
            return BadRequest("Both XML files must be provided.");
        }

        try
        {
            _logger.LogInformation("Reading XML files content.");

            string xmlContent1;
            string xmlContent2;

            using (var reader1 = new StreamReader(filesDto.XmlFile1.OpenReadStream()))
            {
                xmlContent1 = await reader1.ReadToEndAsync();
            }

            _logger.LogInformation("Read first XML file successfully. Size: {Size} characters", xmlContent1.Length);

            using (var reader2 = new StreamReader(filesDto.XmlFile2.OpenReadStream()))
            {
                xmlContent2 = await reader2.ReadToEndAsync();
            }

            _logger.LogInformation("Read second XML file successfully. Size: {Size} characters", xmlContent2.Length);

            _logger.LogInformation("Starting import process.");
            await _importer.ImportFromTwoXmlStringsAsync(xmlContent1, xmlContent2);
            _logger.LogInformation("Import process completed successfully.");

            return Ok(new { message = "Files imported successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import XML files.");
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