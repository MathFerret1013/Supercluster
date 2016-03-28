# Supercluster
A .NET machine learning focusing on [clustering](https://en.wikipedia.org/wiki/Cluster_analysis) and [metric learning](https://en.wikipedia.org/wiki/Similarity_learning).
Currently I am just starting out, but I plan to focus on algorithms that are new (such as the ENN algorithm) as well as old classics (such as DBSCAN). The project is under **heavy** development and I have **many** things planed.

### Philosophy
I am the only developer on the project so I can do it my way.
* **Research first:** All algorithms are self-written. I demand a complete and deep understanding before I start coding. This involves reading many research papers. Many cups of coffee.
* **Correctness:** I am a mathematician by training. Machine Learning is just mathematics. Mathematics must be correct, hence **all** algorithms are rigorously unit tested.
* **Documentation:**  I strive for super-thorough documentation. Admittedly at this stage it is hard to keep up. But after release 1.1. I will re-document the code base.
* **Quality:** Thorough commenting, style-cop compliance, and following good software design principles. Mathematicians have been writing bad code for too long. This stops now. Quality is as important as correctness.
* **Appropriate Efficiency:** Code is optimized where it **needs** to be optimized. Let's be honest. This is C#. I can't compete with C++, but more and more people are using high-level languages for data-science. When an optimization can drastically improve performance (e.g., using k-d-trees, multi-threading) I optimize. When optimization complicates the code for a 1% speed increase, I don't.


###Algorithms
Here is a list of currently supported algorithms:

* [k-NN](https://en.wikipedia.org/wiki/K-nearest_neighbors_algorithm)
* [ENN](http://www.ele.uri.edu/faculty/he/PDFfiles/ENN.pdf) (1st C# implementation)
* [k-Means](https://en.wikipedia.org/wiki/K-means_clustering)

## Next Release

### Algorithms 
* [DBSCAN](https://en.wikipedia.org/wiki/DBSCAN)
* [OPTICS](https://en.wikipedia.org/wiki/OPTICS_algorithm)

### Code
* Refactor of the ENN code.
* ~~A KD-Tree implementation.~~ Done: https://github.com/MathFerret1013/Supercluster.KDTree
* Updating kNN, kMeans and ENN to accept a KD-Tree during construction
