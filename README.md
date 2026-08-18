# Blackbird.io Box

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

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

### Files
- **Search files in folder**: Retrieve a list of files from a specified directory. This action helps you to see all the contents within a folder.
- **Get file information** Obtain detailed information about a specific file, including its name and other metadata.
- **Rename file** Change the name of an existing file to something new.
- **Download file** Download a file from the directory, allowing you to save it locally or use it as needed.
- **Upload file** Upload a new file to a specified directory, making it accessible in your cloud storage.
- **Copy file** Copy a file to a different directory.
- **Move file** Move file to a different parent folder.
- **Delete file** Remove a file from the directory permanently.

### Folders
- **Search folders** List folders in a parent folder.
- **Create folder** Create a new folder within a specified parent directory, helping you organize your files better.
- **Delete folder** Remove a folder and its contents from the directory permanently.
- **Add collaborator to folder** Invite someone to collaborate on a folder by adding them as a collaborator, allowing them to access and manage the contents of the folder.

## Events

### Files

- **On file created**: This event triggers when a file is uploaded to the folder you selected, or to any of its subfolders. It returns the file ID together with the name, path, size and author of the uploaded file.

Every event requires a folder to monitor. Box does not allow monitoring the root folder ("All files"), so pick a specific folder instead.

> **Note**: removing a subscription from Box takes a few moments. If you suspend a bird and activate it again straight away, activation can fail because the previous subscription still exists in Box. Wait at least 30-60 seconds between suspending and reactivating a bird.

## Example

Here is an example of how you can use the Box app in a workflow:

![example](image/README/Example.png)

In this example, the workflow starts with the **On file created** event, which triggers when a file is uploaded to the folder you monitor. Then, the workflow uses the **Download file** action to download the file that was uploaded. In the next step we translate the file via `DeepL` and then upload the translated file back to the Box directory.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
