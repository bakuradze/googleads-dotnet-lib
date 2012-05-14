﻿// Copyright 2011, Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// Author: api.anash@gmail.com (Anash P. Oommen)

using Google.Api.Ads.Common.OAuth.Lib;
using Google.Api.Ads.Dfp.Lib;
using Google.Api.Ads.Dfp.v201204;

using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Google.Api.Ads.Dfp.Examples.OAuth {
  /// <summary>
  /// Code-behind class for GetAllUsers.aspx.
  /// </summary>
  public partial class GetAllUsers : System.Web.UI.Page {
    /// <summary>
    /// The user for creating services and making DFP API calls.
    /// </summary>
    DfpUser user;

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing
    /// the event data.</param>
    void Page_Load(object sender, EventArgs e) {
      user = new DfpUser();
      string url = Request.Url.GetLeftPart(UriPartial.Path);
      user.OAuthProvider = new AdsOAuthNetProvider(DfpService.GetOAuthScope(
          user.Config as DfpAppConfig), url, Session.SessionID);
    }

    /// <summary>
    /// Handles the Click event of the btnAuthorize control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing
    /// the event data.</param>
    protected void OnAuthorizeButtonClick(object sender, EventArgs e) {
      user.OAuthProvider.GenerateAccessToken();
    }

    /// <summary>
    /// Handles the Click event of the btnGetUsers control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing
    /// the event data.</param>
    protected void OnGetUsersButtonClick(object sender, EventArgs e) {
      // Get the UserService.
      UserService userService = (UserService) user.GetService(DfpService.v201204.UserService);

      // Sets defaults for page and Statement.
      UserPage page = new UserPage();
      Statement statement = new Statement();
      int offset = 0;

      DataTable dataTable = new DataTable();
      dataTable.Columns.AddRange(new DataColumn[] {
          new DataColumn("Serial No.", typeof(int)),
          new DataColumn("User Id", typeof(long)),
          new DataColumn("Email", typeof(string)),
          new DataColumn("Role", typeof(string))
      });
      do {
        // Create a Statement to get all users.
        statement.query = string.Format("LIMIT 500 OFFSET {0}", offset);

        // Get users by Statement.
        page = userService.getUsersByStatement(statement);

        if (page.results != null && page.results.Length > 0) {
          int i = page.startIndex;
          foreach (User usr in page.results) {
            DataRow dataRow = dataTable.NewRow();
            dataRow.ItemArray = new object[] {i + 1, usr.id, usr.email, usr.roleName};
            dataTable.Rows.Add(dataRow);
            i++;
          }
        }
        offset += 500;
      } while (offset < page.totalResultSetSize);
      if (dataTable.Rows.Count > 0) {
        UserGrid.DataSource = dataTable;
        UserGrid.DataBind();
      } else {
        Response.Write("No users were found.");
      }
    }


    /// <summary>
    /// Handles the Click event of the btnLogout control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing
    /// the event data.</param>
    protected void OnLogoutButtonClick(object sender, EventArgs e) {
      Session.Clear();
    }

    /// <summary>
    /// Handles the RowDataBound event of the UserGrid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The
    /// <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance
    /// containing the event data.</param>
    protected void OnUserGridRowDataBound(object sender, GridViewRowEventArgs e) {
      if (e.Row.DataItemIndex >= 0) {
        e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
        e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;
        e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Left;
        e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
      }
    }
  }
}
