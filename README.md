# azure-surveillance

This project shows how to build a surveillance system based on Microsoft Azure.

## use case

Pictures taken by a camera are analyzed by use of artifical intelligence. If there was a person detected on the image, an email is sent to the user.
In any case the result is stored in a nosql database (e.g. for later improvements)

## architecture

![picture application architecture](https://github.com/AndiHahn/azure-surveillance/blob/master/doc/architecture.png)

The pictures are uploaded to an azure blob storage. By an event grid system topic connected to this storage an event subscription can be made to publish a 'EventGridMessage' to a storage queue (image-uploaded-queue). An queue triggered azure function (Image processing function) uses cognitive services to detect objects on the image. If a person is detected with a probability of > 70% a message is added to person-detected-queue. Regardless on the image analysis outcome a message is added to processed-image-queue. The queue triggered azure function (Persistence function) stores the result of the image analysis to azure cosmos db. Another queue triggered azure function (Notification function) sends an email to the user with the information of the detected person.

## automation

The repository contains azure pipelines for automated deployment and infrastructure provisioning.

1. Stage: Continuous integration - build and publish azure functions code  
2. Stage: Infrastructure as Code - runs bicep declaration to provision infrastructure
3. Stage: Continuous deployment - release of azure functions to staging slot and swap afterwards

pipelines: contains yaml pipelines for all 3 stages
bicep: contains complete infrastructure in declarative bicep syntax; in bicepconfig.json the objectId of your aad-user and some other configs can be set
terraform: contains partial infrastructure by declarative terraform syntax (not complete!)
