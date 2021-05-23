# Dynamics-CRM-Resource-Synchronization

## What is Dynamics-CRM-Resource-Synchronization?
Extension for Visual Studio higher to version 2017. This repository contains the source code of the extension

## What does the extension do?
It allows you to view and download CRM resource content from custom solutions, compare CRM resource versus resource differences, upload resources after merging differences between different sources (CRM vs Local), publish selected resources to CRM and add default solution resources.

You can indicate the download path dynamically in each download action, by default the path will be that of the selected project in a Visual Studio solution if it exists. You can change this path to a different one as long as Visual Studio has write permissions to download and create resources locally.

Each type of resource can independently configure a path for downloading.

## Authorizations allowed by the extension for Dynamics CRM (CE)
The connection to Dynamics CRM is available for multiple logins: ~~AD~~, OAuth, ~~Certificate~~, ClientSecret, and Office 365 for Dynamcis CRM.

* More documentation : 
  * https://docs.microsoft.com/es-es/dynamics365/customerengagement/on-premises/developer/xrm-tooling/use-connection-strings-xrm-tooling-connect
  * https://docs.microsoft.com/es-es/powerapps/developer/data-platform/walkthrough-register-app-azure-active-directory

## First steps

1. Donwload and install extension in visual studio (>2017) from visual studio marketplace
    - Donwload : https://marketplace.visualstudio.com/items?itemName=Dynamics-CRM-Resource-Synchronization.Dynamics-CRM-Resource-Synchronization
2. Parameters for connection to the Dynamics CRM environment

3. Local resource path configuration 

4. Test connection to environment

5. Donwload solutions managed fron Dynamics CRM

6. Download solution resources

7. Compare, solution conflicts, merge and add resources to solution selected from solution default
