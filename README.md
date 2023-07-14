
# Dot Net Neural Network Fun
This was a project I did a couple years ago to understand the inner workings of a neural network as it trains. It's currently setup to train on the MNIST dataset to identify the handwritten digits. As it trains, you can see the performance and different values of the network: Cost, weights, min/max of weights and bias, etc. 
This wasn't designed for fully training a network for production use. It's mainly for visualizing the training process and experimenting with different hyper parameters.


## Features
- Visualizes the neural network changing over time.
- Implementations of learning features: AdaGrad, Adam, RMSProp, Momentum.
- Implementations of activation functions (also their derivates and 'reverse'): Sigmoid, TanH, ReLU, LeakyReLU, Step.
- Multiple networks with different configurations can be trained all at once. Used for trying to determine what hyper parameters are ideal for a given dataset.
- Multithreaded via Parallel.For().
- Uses an embedded chromium browser (CefSharp) as the UI for a flexible and flowing interface.

## UI Snapshots

Coming soon.


## Installing and Running

Coming soon.



