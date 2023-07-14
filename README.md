
# Dot Net Neural Network Fun
This was a project I did a couple years ago to understand the inner workings of a neural network as it trains. It's currently setup to train on the MNIST dataset to identify the handwritten digits. As it trains, you can see the performance and different values of the network: Cost, weights, min/max of weights and bias, etc. 
This wasn't designed for fully training a network for production use. It's mainly for visualizing the training process and experimenting with different parameters.


## Features
- Visualizes the neural network changing over time.
- Implementations of learning features: AdaGrad, Adam, RMSProp, Momentum.
- Implementations of activation functions (also their derivates and 'reverse'): Sigmoid, TanH, ReLU, LeakyReLU, Step.
- Multiple networks with different configurations can be trained all at once. Used for trying to determine what hyper parameters are ideal for a given dataset.
- Multithreaded via Parallel.For().
- Uses an embedded chromium browser (CefSharp) as the UI for a flexible and flowing interface.

## UI Snapshots

Will include a timelapse GIF of the application running very soon.

### Network Layers and Connections:
<img width="1280" alt="Screenshot 2023-07-14 160521" src="https://github.com/addunn/DotNetNeuralNetFun/assets/43220218/fd3c28c0-cea4-4d3f-83d0-e86fd486366d">

### Network Stats:
<img width="1280" alt="Screenshot 2023-07-14 165308" src="https://github.com/addunn/DotNetNeuralNetFun/assets/43220218/070bfc81-e742-4c50-ac3a-3db55531a51e">
