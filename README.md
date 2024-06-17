# Blackbird.io Box

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

Box is a cloud-based content management and file sharing platform that enables individuals and businesses to store, access, and collaborate on documents and files from anywhere. It offers features like secure file storage, real-time collaboration, and integrations with various productivity tools, enhancing team efficiency and data accessibility.

## Before setting up

Before you can connect you need to make sure that:

- You have a Box account and you have the credentials to access it.

## Connecting

1. Navigate to Apps, and identify the **Box** app. You can use search to find it.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My Plunet connection'.
4. Establish Box connection via OAuth.
5. Click _Connect_.

![connection](image/README/connection.png)

## Actions

- **List directory**: Retrieve a list of items from a specified directory. This action helps you to see all the contents within a folder.
- **Get file information**: Obtain detailed information about a specific file, including its name and other metadata.
- **Rename file**: Change the name of an existing file to something new.
- **Download file**: Download a file from the directory, allowing you to save it locally or use it as needed.
- **Upload file**: Upload a new file to a specified directory, making it accessible in your cloud storage.
- **Delete file**: Remove a file from the directory permanently.
- **Create folder**: Create a new folder within a specified parent directory, helping you organize your files better.
- **Delete directory**: Remove a folder and its contents from the directory permanently.
- **Add collaborator to folder**: Invite someone to collaborate on a folder by adding them as a collaborator, allowing them to access and manage the contents of the folder.

## Events

This section provides event triggers that notify you when certain actions occur within your file and directory management system. These triggers work on a polling mechanism, meaning they periodically check for updates rather than providing immediate notifications.

- **On files created or updated**: This event triggers when any file is created or updated within your directories. It helps you stay informed about changes and new additions to your files.
- **On files deleted**: This event triggers when any file is deleted from your directories. It ensures you are aware of any removed files, helping you keep track of your content.

You can configure the polling interval for these events, choosing how frequently you want the system to check for updates. The interval can range from 5 minutes to 7 days, depending on your preferences and needs.

## Example

Here is an example of how you can use the Box app in a workflow:

![example](image/README/Example.png)

In this example, the workflow starts with the **On files created or updated** event, which triggers when any file is created or updated in your directories. Then, the workflow uses the **Download file** action to download the file that was created or updated. In the next step we translate the file via `DeepL` and then upload the translated file back to the Box directory.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
