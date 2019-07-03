using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;

using System.Diagnostics;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using System.Threading;
using Windows.System.Threading;
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Client;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace ADCReader
{
    public sealed class StartupTask : IBackgroundTask
    {
        #region Constants and variables

        // This should really be read from a config file.
        private string deviceName = "xxxxxxx";

        // These defined GPIO pins on which the movement sensor and status LED will be connected.
        private const int ledPin = 5;
        private GpioPin led;

        private MCP3008 mcp3008;

        // These hold the results of all the sensor data.
        private MCP3008SensorData adcSensorData;

        // Use for configuration of the MCP3008 class voltage formula
        const float ReferenceVoltage = 5.0F; // The MCP3008 works on a 5v reference voltage.

        #endregion

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Using BackgroundTaskDeferral
            // as described in http://aka.ms/backgroundtaskdeferral
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            mcp3008 = new MCP3008(ReferenceVoltage);
            mcp3008.Initialize();

            InitGPIO();

            adcSensorData = new MCP3008SensorData();

            string roomstatus = String.Empty;

            while (true)
            {
                Debug.WriteLine("Reading taken at " + DateTime.UtcNow);

                #region Reading Force Information from Sensor  
                adcSensorData = ReadData();
                Debug.WriteLine("Reading: " + adcSensorData.forceReading);
                #endregion

                //await SendDeviceToCloudMessageAsync(roomstatus, bmpSensorData, adcSensorData);

                await Task.Delay(1000);

            }

           // Once the asynchronous method(s) are done, close the deferral.
           // deferral.Complete();
        }

        #region Private methods
        private void InitGPIO()
        {
            // get the GPIO controller
            var gpio = GpioController.GetDefault();
            
            // return an error if there is no gpio controller
            if (gpio == null)
            {
                led = null;
                Debug.WriteLine("There is no GPIO controller.");
                return;
            }

            Debug.WriteLine("GPIO is Ready. Pin Count = " + gpio.PinCount);

            // set up the LED on the defined GPIO pin
            // and set it to High to turn off the LED
            led = gpio.OpenPin(ledPin);
            led.Write(GpioPinValue.High);
            led.SetDriveMode(GpioPinDriveMode.Output);

            Debug.WriteLine("GPIO pins initialized correctly.");
        }
        private MCP3008SensorData ReadData()
        {
            var MCP3008SensorData = new MCP3008SensorData();
            try
            {
                if (mcp3008 == null)
                {
                    Debug.WriteLine("Force Sensor data is not ready");
                    return MCP3008SensorData;
                }

                // Read from the ADC chip the current values of the two pots and the photo cell.
                MCP3008SensorData.forceReading = mcp3008.ReadADC(0); // Read from analogue input channel 0.

                // convert the ADC readings to voltages to make them more friendly.
                MCP3008SensorData.forceReadingVolts = mcp3008.ADCToVoltage(MCP3008SensorData.forceReading);

                // Let us know what was read in.
                Debug.WriteLine(String.Format("Read value {0}, {1} volts. ", MCP3008SensorData.forceReading, MCP3008SensorData.forceReadingVolts));

                return MCP3008SensorData;
            }
            catch (Exception)
            {
                return MCP3008SensorData;
            }

        }
        //private string CheckForStateValue(eState newState)
        //{
        //    String lightStatus;

        //    switch (newState)
        //    {
        //        case eState.JustRight:
        //            {
        //                lightStatus = JustRightLightString;
        //            }
        //            break;

        //        case eState.TooBright:
        //            {
        //                lightStatus = HighLightString;
        //            }
        //            break;

        //        case eState.TooDark:
        //            {
        //                lightStatus = LowLightString;
        //            }
        //            break;

        //        default:
        //            {
        //                lightStatus = "N/A";
        //            }
        //            break;
        //    }

        //    return lightStatus;
        //}


        private async Task SendDeviceToCloudMessageAsync(string status, BMP280SensorData BMP280SensorData, MCP3008SensorData MCP3008SensorData)
        {
            ConferenceRoomDataPoint conferenceRoomDataPoint = new ConferenceRoomDataPoint()
            {
                DeviceId = deviceName,
                Time = DateTime.UtcNow.ToString("o"),
                RoomTemp = BMP280SensorData.Temperature.ToString(),
                RoomPressure = BMP280SensorData.Pressure.ToString(),
                RoomAlt = BMP280SensorData.Altitude.ToString(),
            };

            if (status == "Occupied")
            {
                conferenceRoomDataPoint.Color = "Red";
            }
            else
            {
                conferenceRoomDataPoint.Color = "Green";
            }



            var jsonString = JsonConvert.SerializeObject(conferenceRoomDataPoint);
            //var jsonStringInBytes = new Message(Encoding.ASCII.GetBytes(jsonString));

            await AzureIoTHub.SendDeviceToCloudMessageAsync(jsonString);
            Debug.WriteLine("{0} > Sending message: {1}", DateTime.UtcNow, jsonString);
        }
        #endregion
    }
}
