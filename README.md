# Resolver
Resolver helps you write *loosely coupled* code that is easy to change, compose and test.
The library is easy to adopt, works with UWP apps and exposes a concise syntax. 

Read more about [Dependency Injection](https://en.wikipedia.org/wiki/Dependency_injection).

## Contents
* **Resolver** Can register a new dependency, build new objects and resolve their dependencies.   
* **Resolvable** Attribute to be used on properties that will automatically be resolved. 

## Usage

Resolver is intended to be used as an [singleton](https://en.wikipedia.org/wiki/Singleton_pattern).
However, that decision belongs to the consumer.
```
var resolver = new Resolver();
```

### Register ###
Once you build you a Resolver, you can use it to register dependencies:
```
ISoundService soundService = new SoundService();
resolver.Register(soundService);
```

### Resolve ###
After you register a property, you can resolve objects that contain properties of the same type as the registered dependencies.

```
class PageViewModel
{
    public ISoundService Sound { get; set; }
}

var viewModel = resolver.Resolve<PageViewModel>();
```
At this point, ```Sound``` property of the ```viewModel``` object has been populated with ```soundService``` object.

**Notice**: ```Resolve``` **only** creates new objects, but **no new dependencies**.

E.g.:
```
var viewModel1 = resolver.Resolve<PageViewModel>();
var viewModel2 = resolver.Resolve<PageViewModel>();
```
```viewModel1.Sound``` is equal to ```viewModel2.Sound``` by pointing to the same reference ```soundService```.

### [Resolvable] ###
```Resolvable``` attribute can be use on any property that will have to be instantiated at runtime.

```
class ControlViewModel
{
}

class NavigationViewModel
{
    [Resolvable]
    public ControlViewModel Control { get; set; }
}

var navigation = resolver.Resolve<NavigationViewModel>();
```
At this point ```navigation.Control``` has been instantiated and can therefore be used.

### Registered dependencies vs. ```[Resolvable]``` properties ###
As you have seen above, the main difference between a registered dependency and a ```[Resolvable]``` property is their lifetime and their relation to their owner.
* Registered dependencies have a longer lifetime than the objects that consume them. E.g.: a SoundService can outlive different app pages.
* ```[Resolvable]``` properties are have a shorter or equal lifetime than their owner's. E.g.: a property CreateAccountView only makes sense on a Start page, therefore there is no need to keep it around longer that the Start page.

Finally, registered dependencies and ```[Resolvable]``` can be used in the same class, and the ```Resolve``` method with figure out how to handle each according to the rules mentioned above. Furthermore, the newly instantiated ```[Resolvable]``` property follows the same ```Resolve``` logic, meaning that any further dependencies it has will be resolved as well (either registered or ```[Resolvable]```)

```
class ControlViewModel
{
    public ISoundService Sound { get; set; }
}

class NavigationViewModel
{
    [Resolvable]
    public ControlViewModel Control { get; set; }

    public ISoundService Sound { get; set; }
}

var navigation = resolver.Resolve<NavigationViewModel>();
```
At this point ```navigation.Control``` has been instantiated, and both ```navigation.Sound``` and ``` navigation.Control.Sound``` point to the same reference, ```soundService```.

### Performance ###
**Resolver** uses [Reflection](https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/reflection) to inspect types and create objects. While Reflection is a powerful library, it does come with some performance penalty.
There are numerous resources where you can find more details about how fast Refection build objects, and Resolver's performance will follow Reflection's performance.

Here are tests performed on a DEBUG build that create **10.000** objects using instantiation and ```Resolve```. 

* Scenario 1: No registered or ```[Resolvable]``` properties

|Instantiation|```Resolve```|
|:-----------:|:-----------:|
|< 1 ms       | 20 ms       |

* Scenario 2: registered dependencies

||Instantiation|```Resolve```|
|:-:|:-:|:-:|
|1 dependency|< 1 ms|5 ms|
|10 dependencies|4 ms|175 ms|

* Scenario 3: ```[Resolvable]``` properties

||Instantiation|```Resolve```|
|:-:|:-:|:-:|
|1 dependency|< 1 ms|50 ms|
|10 dependencies|4 ms|400 ms|

The performance penalty is noticeable when creating a large number of objects, especially when using ```[Resolvable]``` properties.
That is because Reflection needs to assign and / or instantiate ```(number of objects) * (number of dependencies)```.

In real world scenarios, general purpose apps tend to have smaller object hierarchies making the use of DI acceptable. In most cases even objects with a complex hierarchy or multiple levels of child objects can be resolved in less than 1 ms.

While analyzing CPU performance in real world app usage the cumulative CPU time spend by Resolver accounts for less than 1% of the total time spend on high activity scenarios that require creating new objects.     

## Why use Resolver
Several other platforms implement Dependency Injection for .NET.
Some of the most popular ones include: [Ninject](http://www.ninject.org/), [Castle Windsor](https://github.com/castleproject/Windsor).

**Resolver** is meant to be simple and easy to understand.

The ```Resolver``` class contains less than 160 lines of code. 
```GetNewObject``` and ```ResolveObjectProperties```, the code methods of the framework represent less than half the implementation of the ```Resolver``` class.

The conciseness of the implementation makes it a good candidate for consumers looking for either a lightweight or an easy to understand DI implementation.

## Apps using this library
I wrote **Resolver** out of necessity, after passing dependencies via constructors got out of hand and after I learned about Dependency Injection.

It has since simplified my code, allowed me to focus on the actual logic instead of infrastructure code and ultimately helped me ship faster and better apps.

I used this library in several of my games, check them out in Windows Store:

* [Jigsaw Puzzle Frenzy](https://www.microsoft.com/store/apps/9wzdncrddqbm)
* [Hexa Word Search](https://www.microsoft.com/store/apps/9mtxw2nrnjf1)
* [Quiz for Geeks](https://www.microsoft.com/store/apps/9wzdncrddqbg)
* [Fun with Words](https://www.microsoft.com/store/apps/9nblgggzpgrt)
* [Easy Peasy Puzzles](https://www.microsoft.com/store/apps/9wzdncrddj8g)