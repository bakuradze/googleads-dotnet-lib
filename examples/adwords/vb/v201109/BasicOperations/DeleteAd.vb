' Copyright 2011, Google Inc. All Rights Reserved.
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'     http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.

' Author: api.anash@gmail.com (Anash P. Oommen)

Imports Google.Api.Ads.AdWords.Lib
Imports Google.Api.Ads.AdWords.v201109

Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace Google.Api.Ads.AdWords.Examples.VB.v201109
  ''' <summary>
  ''' This code example deletes an ad using the 'REMOVE' operator. To list ads,
  ''' run GetTextAds.vb.
  '''
  ''' Tags: AdGroupAdService.mutate
  ''' </summary>
  Public Class DeleteAd
    Inherits ExampleBase
    ''' <summary>
    ''' Main method, to run this code example as a standalone application.
    ''' </summary>
    ''' <param name="args">The command line arguments.</param>
    Public Shared Sub Main(ByVal args As String())
      Dim codeExample As ExampleBase = New DeleteAd
      Console.WriteLine(codeExample.Description)
      Try
        codeExample.Run(New AdWordsUser, codeExample.GetParameters, Console.Out)
      Catch ex As Exception
        Console.WriteLine("An exception occurred while running this code example. {0}", _
            ExampleUtilities.FormatException(ex))
      End Try
    End Sub

    ''' <summary>
    ''' Returns a description about the code example.
    ''' </summary>
    Public Overrides ReadOnly Property Description() As String
      Get
        Return "This code example deletes an ad using the 'REMOVE' operator. To list ads, " & _
            "run GetTextAds.vb."
      End Get
    End Property

    ''' <summary>
    ''' Gets the list of parameter names required to run this code example.
    ''' </summary>
    ''' <returns>
    ''' A list of parameter names for this code example.
    ''' </returns>
    Public Overrides Function GetParameterNames() As String()
      Return New String() {"ADGROUP_ID", "AD_ID"}
    End Function

    ''' <summary>
    ''' Runs the code example.
    ''' </summary>
    ''' <param name="user">The AdWords user.</param>
    ''' <param name="parameters">The parameters for running the code
    ''' example.</param>
    ''' <param name="writer">The stream writer to which script output should be
    ''' written.</param>
    Public Overrides Sub Run(ByVal user As AdWordsUser, ByVal parameters As  _
        Dictionary(Of String, String), ByVal writer As TextWriter)
      ' Get the AdGroupAdService.
      Dim adGroupAdService As AdGroupAdService = user.GetService( _
          AdWordsService.v201109.AdGroupAdService)

      Dim adGroupId As Long = Long.Parse(parameters("ADGROUP_ID"))
      Dim adId As Long = Long.Parse(parameters("AD_ID"))

      ' Since we do not need to update any ad-specific fields, it is enough to
      ' create the base type.
      Dim ad As New Ad
      ad.id = adId

      ' Create the ad group ad.
      Dim adGroupAd As New AdGroupAd
      adGroupAd.adGroupId = adGroupId

      adGroupAd.ad = ad

      ' Create the operation.
      Dim operation As New AdGroupAdOperation
      operation.operand = adGroupAd
      operation.operator = [Operator].REMOVE

      Try
        ' Delete the ad.
        Dim retVal As AdGroupAdReturnValue = adGroupAdService.mutate( _
            New AdGroupAdOperation() {operation})

        If ((Not retVal Is Nothing) AndAlso (Not retVal.value Is Nothing) AndAlso _
            (retVal.value.Length > 0)) Then
          Dim deletedAdGroupAd As AdGroupAd = retVal.value(0)
          writer.WriteLine("Ad with id = ""{0}"" and type = ""{1}"" was deleted.", _
              deletedAdGroupAd.ad.id, deletedAdGroupAd.ad.AdType)
        Else
          writer.WriteLine("No ads were deleted.")
        End If
      Catch ex As Exception
        Throw New System.ApplicationException("Failed to delete ad.", ex)
      End Try
    End Sub
  End Class
End Namespace
