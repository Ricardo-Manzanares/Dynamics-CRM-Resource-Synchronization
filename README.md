# Dynamics-CRM-Resource-Synchronization
Extension for Visual Studio higher to version 2017.

It allows you to view and download CRM resource content from custom solutions, compare CRM resource versus resource differences, upload resources after merging differences between different sources (CRM vs Local), publish selected resources to CRM and add default solution resources.

You can indicate the download path dynamically in each download action, by default the path will be that of the selected project in a Visual Studio solution if it exists. You can change this path to a different one as long as Visual Studio has write permissions to download and create resources locally.

Each type of resource can independently configure a path for downloading.

The connection to Dynamics CRM is available for multiple logins: AD, OAuth, Certificate, ClientSecret, and Office 365 for Dynamcis CRM.
