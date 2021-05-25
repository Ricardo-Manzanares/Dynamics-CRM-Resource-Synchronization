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

## Change history

# 0.2
Applied improvements :
1. Synchronized navigation in the resource conflicts window is disabled between CRM resource content or Local resource content and conflicts. It caused slow navigation.
2. Update environment and user data on the main screen after updating connection data. 
3. Find local resources with the name of the resource and not the full path of the resource in Dynamics CRM.
4. Navigate between conflicts for a resource by selecting the conflict text and avoid manual scrolling to find the conflict

# 0.1
Publish initial :
1. Initial publication containing:
2. View Managed Dynamics CRM Solutions.
3. View the resources of a Dynamics CRM managed solution.
4. Add new resources from the Dynamics CRM default solution to the selected managed solution.
5. Download, upload and publish resources in Dynamics CRM.
6. Filter resources by resource name and type.
7. Compare Differences Between Dynacis CRM Resource and Local.
8. Connection to Dynamics CRM environments is allowed with: OAuth, ClientSecret and Office365.
9. Establish comparison paths for each type of resource.
