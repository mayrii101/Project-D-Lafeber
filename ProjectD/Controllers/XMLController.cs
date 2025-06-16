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
    public async Task<IActionResult> UploadXmlFile([FromForm] XmlFileUploadDto fileDto)
    {
        _logger.LogInformation("Received request to upload a single XML file.");

        if (fileDto.XmlFile == null)
        {
            _logger.LogWarning("Upload request is missing the XML file.");
            return BadRequest("XML file must be provided.");
        }

        try
        {
            string xmlContent;
            using (var reader = new StreamReader(fileDto.XmlFile.OpenReadStream()))
            {
                xmlContent = await reader.ReadToEndAsync();
            }

            _logger.LogInformation("Read XML file successfully. Size: {Size} characters", xmlContent.Length);
            _logger.LogInformation("Starting import process.");

            await _importer.ImportFromXmlStringAsync(xmlContent);

            _logger.LogInformation("Import process completed successfully.");
            return Ok(new { message = "File imported successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import XML file.");
            return StatusCode(500, "Internal server error while importing XML file.");
        }
    }
}

// DTO for model binding a single uploaded file
public class XmlFileUploadDto
{
    [FromForm(Name = "xmlFile")]
    public IFormFile XmlFile { get; set; }
}