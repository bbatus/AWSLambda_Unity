using UnityEngine;
using UnityEngine.UI;
using Amazon.Lambda;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon;
using System.Text;
using Amazon.Lambda.Model;
using static System.Net.Mime.MediaTypeNames;
using UnityEditor.PackageManager;
using TMPro;

namespace AWSSDK.Examples
{
    public class LambdaExample : MonoBehaviour
    {
        public string IdentityPoolId = "";
        public string CognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;
        public TextMeshProUGUI counttext;

        private RegionEndpoint _CognitoIdentityRegion
        {
            get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
        }
        public string LambdaRegion = RegionEndpoint.USEast1.SystemName;
        private RegionEndpoint _LambdaRegion
        {
            get { return RegionEndpoint.GetBySystemName(LambdaRegion); }
        }


        public Button InvokeButton = null;
        public Button ListFunctionsButton = null;
        public InputField FunctionNameText = null;
        public InputField EventText = null;
        public TextMeshProUGUI ResultText = null;

        void Start()
        {
            UnityInitializer.AttachToGameObject(this.gameObject);
            InvokeButton.onClick.AddListener(() => { Invoke(); });
            ListFunctionsButton.onClick.AddListener(() => { ListFunctions(); });
        }

        #region private members

        private IAmazonLambda _lambdaClient;
        private AWSCredentials _credentials;

        private AWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
                return _credentials;
            }
        }

        private IAmazonLambda Client
        {
            get
            {
                if (_lambdaClient == null)
                {
                    _lambdaClient = new AmazonLambdaClient(Credentials, _LambdaRegion);
                }
                return _lambdaClient;
            }
        }

        #endregion

        #region Invoke
        /// <summary>
        /// Example method to demostrate Invoke. Invokes the Lambda function with the specified
        /// function name (e.g. helloWorld) with the parameters specified in the Event JSON.
        /// Because no InvokationType is specified, the default 'RequestResponse' is used, meaning
        /// that we expect the AWS Lambda function to return a value.
        /// </summary>
        public void Invoke()
        {
            ResultText.text = "Invoking '" + FunctionNameText.text + " function in Lambda... \n";
            Client.InvokeAsync(new Amazon.Lambda.Model.InvokeRequest()
            {
                FunctionName = FunctionNameText.text,
                Payload = EventText.text
            },
            (responseObject) =>
            {
                ResultText.text += "\n";
                if (responseObject.Exception == null)
                {
                    ResultText.text += Encoding.ASCII.GetString(responseObject.Response.Payload.ToArray()) + "\n";
                }
                else
                {
                    ResultText.text += responseObject.Exception + "\n";
                }
            }
            );
        }

        #endregion

        #region List Functions
        /// <summary>
        /// Example method to demostrate ListFunctions
        /// </summary>
        public void ListFunctions()
        {
            ResultText.text = "Listing all of your Lambda functions... \n";
            Client.ListFunctionsAsync(new Amazon.Lambda.Model.ListFunctionsRequest(),
            (responseObject) =>
            {
                ResultText.text += "\n";
                if (responseObject.Exception == null)
                {
                    ResultText.text += "Functions: \n";
                    foreach (FunctionConfiguration function in responseObject.Response.Functions)
                    {
                        ResultText.text += "    " + function.FunctionName + "\n";
                    }
                }
                else
                {
                    ResultText.text += responseObject.Exception + "\n";
                }
            }
            );
        }

        public async void OnButtonClick()
        {
            Client.ListFunctionsAsync(new Amazon.Lambda.Model.ListFunctionsRequest(),
                (responseObject) =>
                {
                    counttext.text += "\n";
                    if (responseObject.Exception == null)
                    {
                        int functionCount = responseObject.Response.Functions.Count;
                        counttext.text += "Total Lambda Functions: " + functionCount + "\n";
                    }
                    else
                    {
                        counttext.text += responseObject.Exception + "\n";
                    }
                }
                );
        }
    }

    #endregion
}


   
