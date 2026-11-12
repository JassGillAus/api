using NUnit.Framework;
using NUnit.Framework.Api;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace api;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PlaywrightTest
{
     string _baseURL = "https://jsonplaceholder.typicode.com";

    /// <summary>
    /// This Test sends a Get Request to fetch Post Record and Check for Response
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task GetParcel_ValidId_Returns200()
    {



        // initalize Playwright
        var ContextSettings = new APIRequestNewContextOptions();
        ContextSettings.BaseURL = _baseURL;


        using var ourPlaywright = await Microsoft.Playwright.Playwright.CreateAsync();

        // create API Context
        await using var ourRequest = await ourPlaywright.APIRequest.NewContextAsync(ContextSettings);

        IAPIResponse ourResponse = await ourRequest.GetAsync("/posts/1");

        await Expect(ourResponse).ToBeOKAsync();

        // Nunit Assertion
        Assert.That(ourResponse.Status, Is.EqualTo(200));

        string ourResponseBody = await ourResponse.TextAsync();

        using JsonDocument ourBodyJSon = JsonDocument.Parse(ourResponseBody);

        Console.WriteLine(ourResponseBody);


        int ourtotalRecords = ourBodyJSon.RootElement.GetPropertyCount();
        Console.WriteLine("Total Properties Elements found: " + ourtotalRecords);

        int ourFirstRecordId = ourBodyJSon.RootElement.GetProperty("userId").GetInt32();
        Console.WriteLine("Our UserId is : " + ourFirstRecordId);


        Assert.Multiple(() =>
        {
            Assert.That(ourtotalRecords, Is.GreaterThan(0));
            Assert.That(ourFirstRecordId, Is.EqualTo(1));

        }
        );

        string ourHeadersContent = ourResponse.Headers["content-type"];
        Console.WriteLine(ourHeadersContent);

        
    }


    /// <summary>
    /// Method to Create a New Record Using POST Request
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task CreateParcel_ValidData_Returns201()
    {
        APIRequestNewContextOptions ourClientOption = new APIRequestNewContextOptions();
        ourClientOption.BaseURL = _baseURL;

        using var ourPlaywrightContext = await Microsoft.Playwright.Playwright.CreateAsync();
        await using var ourRequestContext = await ourPlaywrightContext.APIRequest.NewContextAsync(ourClientOption);

        APIRequestContextOptions ourRequestOptions = new APIRequestContextOptions();

        var ourPayLoad = new
        {
            title = "foo",
            body = "bar",
            userId = 1,
        };
        ourRequestOptions.DataObject = ourPayLoad;

        IAPIResponse ourResponse = await ourRequestContext.PostAsync("/posts", ourRequestOptions);
        await Expect(ourResponse).ToBeOKAsync();
        JsonElement? ourResponseBodyInJson = await ourResponse.JsonAsync();
        
        Console.WriteLine(ourResponseBodyInJson);

        string? ourTitle = ourResponseBodyInJson?.GetProperty("title").GetString();
        string? ourBody = ourResponseBodyInJson?.GetProperty("body").GetString();

        Console.WriteLine("Record created and response contains Title: "+ ourTitle);
        Console.WriteLine("Record created and response contains Body: "+ ourBody);
        

        Assert.That(ourTitle, Is.EqualTo(ourPayLoad.title));
        Assert.That(ourBody, Is.EqualTo(ourPayLoad.body));

         

    }



    /// - Negative Test
    /// <summary>
    /// Negative tests 
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task GetParcel_ValidId_ContractTesting()
    {



        // initalize Playwright
        var ContextSettings = new APIRequestNewContextOptions();
        ContextSettings.BaseURL = _baseURL;


        using var ourPlaywright = await Microsoft.Playwright.Playwright.CreateAsync();

        // create API Context
        await using var ourRequest = await ourPlaywright.APIRequest.NewContextAsync(ContextSettings);

        IAPIResponse ourResponse = await ourRequest.GetAsync("/posts/1");

        //await Expect(ourResponse).ToBeOKAsync();

        Console.WriteLine("Status returned is : "+ ourResponse.Status);
        //Console.WriteLine("Body Returned is :"+ ourResponse.TextAsync());
        // Nunit Assertion
        Assert.That(ourResponse.Status, Is.EqualTo(200));
        //Assert.That(ourResponse.Ok, Is.False);
        
        

        string ourResponseBody = await ourResponse.TextAsync();

        using JsonDocument ourBodyJSon = JsonDocument.Parse(ourResponseBody);


        Console.WriteLine("TEst 3: Reponse Body: "+ourResponseBody);


        JsonElement ourRootElement = ourBodyJSon.RootElement;
        Console.WriteLine("Type is :"+ ourRootElement.ValueKind.ToString());

        // Data Type Checks
        Assert.That(ourRootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));
        
        // Required Field Checks
        Assert.That(ourRootElement.TryGetProperty("id", out var id), Is.True);

        Console.WriteLine("Test 3: Id from response " + id);
       
        //Field is Not Empty Checks
        Assert.That(ourRootElement.TryGetProperty("title", out var title), Is.True);

        Console.WriteLine("Test 3: title from response " + title);

       // Assert.That(title, Is.Not.Null);
                Assert.That(title.GetString(), Is.Not.Empty);
       
    }

}
