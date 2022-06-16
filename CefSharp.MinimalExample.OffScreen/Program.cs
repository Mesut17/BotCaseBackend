// Copyright © 2010-2021 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp.OffScreen;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CefSharp.MinimalExample.OffScreen
{
    /// <summary>
    /// CefSharp.OffScreen Minimal Example
    /// </summary>
    public static class Program
    {
        static readonly string clickScript = "document.getElementsByClassName('header-signin')[0].click()";
        static readonly string signInClick = "document.getElementsByClassName('session-form')[0].getElementsByClassName('sds-button')[0].click()";
        static readonly string signInSetEmail = "document.getElementById('email').value = 'johngerson808@gmail.com';";
        static readonly string signInSetPassword = "document.getElementById('password').value = 'test8008';";
        //static readonly string carTypeSelected = "document.getElementById('make-model-search-stocktype').value = 'used'";
        //static readonly string makeTypeSelected = "document.getElementById('makes').value = 'tesla'";
        //static readonly string modelTypeSelected = "document.getElementById('models').value = 'tesla-model_s'";
        //static readonly string priceSelected = "document.getElementById('make-model-max-price').value = '100000'";
        //static readonly string distanceSelected = "document.getElementById('make-model-maximum-distance').value = 'all'";
        //static readonly string zipSetValue = "document.getElementById('make-model-zip').value = '94596'";
        //static readonly string searchClick = "document.querySelectorAll('button[data-linkname=\"search-used-make\"]')[0].click();";
        static readonly string search = "https://www.cars.com/shopping/results/?page={0}&page_size=20&list_price_max=100000&makes[]=tesla&maximum_distance=all&models[]={1}&stock_type=used&zip=94596";
        //static readonly string searchData = "document.getElementById('search-live-content').getAttribute('data-site-activity')";
        //static readonly string scrollToBottom = "window.scrollTo(0, document.body.scrollHeight)";
        //static readonly string goSecondPageClick = "document.querySelectorAll('[phx-value-page=\"2\"]')[0].click()";
        static readonly string pageData = @"(function(){
        var a = document.getElementById('search-live-content').getAttribute('data-site-activity');
        return a;})()";
        static readonly string specificCarDataDetail = "https://www.cars.com/vehicledetail/3a85b09d-c91f-43dd-bced-4994a612f589/";
        static readonly string carBasicsData = @"(function(){let nodeList = document.getElementsByClassName('fancy-description-list')[0].childNodes;let basic = {};
                                                let basics = [];
                                                for(let i = 0 ; i<nodeList.length ; i++)
                                                    {
                                                        let node = nodeList[i];
                                                        if(node.nodeName === 'DT')
                                                        {
                                                            basic = {
                                                                name:node.innerText
                                                    }
                                                }
                                                if (node.nodeName === 'DD')
                                                {
                                                    basic = {
                                                        ...basic,
                                                                value: node.innerText
                                                            }
                                                    basics.push(basic);
                                                
                                                }
                                                    }
                                                return JSON.stringify(basics);})();";
        static readonly string carFeaturesData = @"(function(){let nodeList = document.getElementsByClassName('fancy-description-list')[1].childNodes;
                                                let feature = {};
                                                let features = [];
                                                for(let i = 0 ; i<nodeList.length ; i++)
                                                    {
                                                        let node = nodeList[i];
                                                        if(node.nodeName === 'DT')
                                                        {
                                                            feature = {
                                                                name:node.innerText
                                                            }
                                                        }
                                                        if (node.nodeName === 'DD')
                                                        {
                                                            feature = {
                                                                ...feature,
                                                                  value: node.innerText
                                                             }
                                                            features.push(feature);                                                       
                                                        }
                                                     }
                                                return JSON.stringify(features);})();";
        static readonly string carHighlightClick = "document.getElementsByClassName('vehicle-badging')[0].click()";
        static readonly string carHighlightData = @"(function(){
          let highlights = [];
          let modal = document.getElementsByClassName('sds-modal sds-modal-visible')[0]
          let highlightElements = modal.querySelectorAll('ul>li>div');
          
          for(let i = 0 ; i<highlightElements.length ; i++)
          {
              let highlightElement = highlightElements[i];
              let headerElement = highlightElement.querySelector('div');
              let contentElement = highlightElement.querySelector('p');
              let iconElement = highlightElement.querySelector('use');
              let highlight = 
              {
                  name : headerElement.innerText,
                  value : contentElement.innerText,
                  icon : iconElement ? iconElement.getAttribute('xlink:href') : null
                  
              };
              highlights.push(highlight);
          }
            return JSON.stringify(highlights);
         })()";

        /// <summary>
        /// Asynchronous demo using CefSharp.OffScreen
        /// Loads google.com, uses javascript to fill out the search box then takes a screenshot which is opened
        /// in the default image viewer.
        /// For a synchronous demo see <see cref="MainSync(string[])"/> below.
        /// </summary>
        /// <param name="args">args</param>
        /// <returns>exit code</returns>
        public static int Main(string[] args)
        {
#if ANYCPU
            //Only required for PlatformTarget of AnyCPU
            CefRuntime.SubscribeAnyCpuAssemblyResolver();
#endif

            const string testUrl = "https://www.cars.com/";

            //Console.WriteLine("This example application will load {0}, take a screenshot, and save it to your desktop.", testUrl);
            //Console.WriteLine("You may see Chromium debugging output, please wait...");
            //Console.WriteLine();


            AsyncContext.Run(async delegate
            {
                var settings = new CefSettings()
                {
                    //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                    CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
                };

                //Perform dependency check to make sure all relevant resources are in our output directory.
                var success = await Cef.InitializeAsync(settings, performDependencyCheck: true, browserProcessHandler: null);

                if (!success)
                {
                    throw new Exception("Unable to initialize CEF, check the log file.");
                }

                // Create the CefSharp.OffScreen.ChromiumWebBrowser instance
                using (var browser = new ChromiumWebBrowser(testUrl))
                {
                    var initialLoadResponse = await browser.WaitForInitialLoadAsync();

                    if (!initialLoadResponse.Success)
                    {
                        throw new Exception(string.Format("Page load failed with ErrorCode:{0}, HttpStatusCode:{1}", initialLoadResponse.ErrorCode, initialLoadResponse.HttpStatusCode));
                    }

                    await browser.EvaluateScriptAsync(clickScript);
                    await Task.Delay(500);
                    //await browser.WaitForNavigationAsync(TimeSpan.FromSeconds(1));
                    await browser.EvaluateScriptAsync(signInSetEmail + signInSetPassword);
                    await browser.EvaluateScriptAsync(signInClick);
                    await Task.Delay(500);
                    await browser.LoadUrlAsync(string.Format(search, 1, "tesla-model_s"));
                    //await browser.WaitForNavigationAsync(TimeSpan.FromSeconds(5));
                    //await browser.WaitForSelectorAsync("search-live-content"); Buraya çalışılacak.
                    await Task.Delay(5000);
                    var page1 = await browser.EvaluateScriptAsync(pageData);
                    await browser.LoadUrlAsync(string.Format(search, 2, "tesla-model_s"));
                    //await browser.WaitForNavigationAsync(TimeSpan.FromSeconds(5));
                    await Task.Delay(5000);
                    var page2 = await browser.EvaluateScriptAsync(pageData);
                    await browser.LoadUrlAsync(specificCarDataDetail);
                    await Task.Delay(5000);
                    //await browser.WaitForNavigationAsync(TimeSpan.FromSeconds(5));
                    var carBasicsDataJson = await browser.EvaluateScriptAsync(carBasicsData);
                    var carFeaturesDataJson = await browser.EvaluateScriptAsync(carFeaturesData);
                    await browser.EvaluateScriptAsync(carHighlightClick);
                    await Task.Delay(500);
                    var carHighlightDataJson = await browser.EvaluateScriptAsync(carHighlightData);
                    var dataBasicPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "carBasicsDataJson.json");
                    var dataFeaturePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "carFeaturesDataJson.json");
                    var datacarHighlightPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "carHighlightDataJson.json");
                    File.WriteAllText(dataBasicPath, carBasicsDataJson.Result as string??"");
                    File.WriteAllText(dataFeaturePath, carFeaturesDataJson.Result as string ?? "");
                    File.WriteAllText(datacarHighlightPath, carHighlightDataJson.Result as string ?? "");
                    await browser.LoadUrlAsync(string.Format(search, 1, "tesla-model_x"));
                    //await browser.WaitForNavigationAsync(TimeSpan.FromSeconds(5));
                    //await browser.WaitForSelectorAsync("search-live-content"); Buraya çalışılacak.
                    await Task.Delay(5000);
                    var page11 = await browser.EvaluateScriptAsync(pageData);
                    await browser.LoadUrlAsync(string.Format(search, 2, "tesla-model_x"));
                    //await browser.WaitForNavigationAsync(TimeSpan.FromSeconds(5));
                    await Task.Delay(5000);
                    var page22 = await browser.EvaluateScriptAsync(pageData);
                    await browser.LoadUrlAsync(specificCarDataDetail);
                    await Task.Delay(5000);
                    //await browser.WaitForNavigationAsync(TimeSpan.FromSeconds(5));
                    var carBasicsDataJson2 = await browser.EvaluateScriptAsync(carBasicsData);
                    var carFeaturesDataJson2 = await browser.EvaluateScriptAsync(carFeaturesData);
                    await browser.EvaluateScriptAsync(carHighlightClick);
                    await Task.Delay(500);
                    var carHighlightDataJson2 = await browser.EvaluateScriptAsync(carHighlightData);
                    var dataBasicPath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "carBasicsDataJson2.json");
                    var dataFeaturePath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "carFeaturesDataJson2.json");
                    var datacarHighlightPath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "carHighlightDataJson2.json");
                    File.WriteAllText(dataBasicPath2, carBasicsDataJson2.Result as string ?? "");
                    File.WriteAllText(dataFeaturePath2, carFeaturesDataJson2.Result as string ?? "");
                    File.WriteAllText(datacarHighlightPath2, carHighlightDataJson2.Result as string ?? "");
                    //await browser.EvaluateScriptAsync(carTypeSelected); 
                    //await browser.EvaluateScriptAsync(makeTypeSelected);
                    //await Task.Delay(500);
                    //await browser.EvaluateScriptAsync(modelTypeSelected);
                    //await browser.EvaluateScriptAsync(priceSelected); 
                    //await browser.EvaluateScriptAsync(distanceSelected); 
                    //await browser.EvaluateScriptAsync(zipSetValue);
                    //await Task.Delay(1000);
                    //await browser.EvaluateScriptAsync(searchClick);
                    //await Task.Delay(3000);
                    ////var page1 =  await browser.EvaluateScriptAsync(searchData1);
                    //await browser.EvaluateScriptAsync(scrollToBottom);
                    //await browser.EvaluateScriptAsync(goSecondPageClick);
                    //await Task.Delay(5000);
                    //var page2 = await browser.EvaluateScriptAsync(searchData1);
                    //await Task.Delay(500);
                    ////Give the browser a little time to render
                    //await Task.Delay(500);
                    // Wait for the screenshot to be taken.
                    var bitmapAsByteArray = await browser.CaptureScreenshotAsync();

                    // File path to save our screenshot e.g. C:\Users\{username}\Desktop\CefSharp screenshot.png
                    var screenshotPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CefSharp screenshot.png");

                    Console.WriteLine();
                    Console.WriteLine("Screenshot ready. Saving to {0}", screenshotPath);

                    File.WriteAllBytes(screenshotPath, bitmapAsByteArray);

                    Console.WriteLine("Screenshot saved. Launching your default image viewer...");

                    // Tell Windows to launch the saved image.
                    Process.Start(new ProcessStartInfo(screenshotPath)
                    {
                        // UseShellExecute is false by default on .NET Core.
                        UseShellExecute = true
                    });

                    Console.WriteLine("Image viewer launched. Press any key to exit.");
                }

                // Wait for user to press a key before exit
                Console.ReadKey();

                // Clean up Chromium objects. You need to call this in your application otherwise
                // you will get a crash when closing.
                Cef.Shutdown();
            });

            return 0;
        }

      
       
    }
}
