![ThingModel](https://raw.github.com/SINTEF-9012/ThingModel/master/Documentation/Logo.png)
==========

ThingModel is simple and like a new wheel.

It shares data and models in realtime over the network for multiple devices. Mainly iPads, PC and a PixelSense table.

ThingModel does not need compilation, it does not include a DSL either and everything is done during runtime. Also, it is not dependant to a model, you can build the model on the fly and the connected applications can learn it during their executions.

If you are looking for innovative products about models, you should also take a look at [ThingML](http://thingml.org/) or [Kevoree](http://kevoree.org/kmf/).

## Model

![Model](https://raw2.github.com/SINTEF-9012/ThingModel/master/Documentation/ThingModel.png)


## Light network synchronization

 * Clients and servers over WebSockets
 * Light serialization with Protocol Buffers
 * Diff algorithm, only send changes
 * String cache for smaller messages

## Multi-plateform

 * __C# .Net__
  * For the PixelSense table
  * And because C# is cool
 * __JavaScript__ (and TypeScript)
  * Recent HTML5 browsers (need WebSocket support)
  * Can work in Node.JS
 * __Java__
  * Mainly for Minecraft
  * Or a HLA bridge

## Bugs

ThingModel is tested and used almost every days and it works, but *it's not stable*.

## Installation

__Nuget__: ```Install-Package ThingModel```

__Bower__: ```bower install ThingModel```

__Maven__: *need motivation*

Otherwise: [__Download__](https://github.com/SINTEF-9012/ThingModel/archive/master.zip)

## Example (in CSharp)

```csharp

// Declare the rabbit type
var typeRabbit = BuildANewThingType.Named("rabbit")
				.WhichIs("Just a rabbit")
				.ContainingA.String("name")
				.AndA.Location("localization")
				.AndA.NotRequired.Double("speed")
				.AndAn.Int("nbChildren", "Number of children");
				
// Create a rabbit instance
var rabbit = BuildANewThing.As(typeRabbit)
				.IdentifiedBy("ab548")
				.ContainingA.String("name", "Roger")
				.AndA.Location("localization", new Location.Point(42,51))
				.AndAn.Int("nbChildren", 12);

// The wharehouse stores the objects
var wharehouse = new Wharehouse();

// Create a websocket client
var client = new Client("Emitter", "ws://localhost:1234", wharehouse);

// Register the rabbit
wharehouse.RegisterThing(rabbit);

// Send the changes to the server
client.Send();
```

```csharp
var wharehouse = new Wharehouse();
var client = new Client("Receiver", "ws://localhost:1234", wharehouse);

wharehouse.Events.OnNew += (sender, args) => {
    // The GetProperty call is a bit ugly, it will change
    Console.WriteLine(args.Thing.GetProperty<Property.String>("name").Value);
};
```
