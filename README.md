# Supercluster
A .NET machine learning focusing on [clustering](https://en.wikipedia.org/wiki/Cluster_analysis) and [metric learning](https://en.wikipedia.org/wiki/Similarity_learning).
Currently I am just starting out, but I plan to focus on algorithms that are new (such as the ENN algorithm) as well as old classics (such as DBSCAN). The project is under heavy development and I have **many** things planed.

### Philosophy
I am the only developer on the project so I can do it my way.
* **Research first:** All algorithms are self-written. I demand a complete a deep understanding before I start coding. This involves reading many research papers. Many cups of coffee.
* **Correctness:** I am a mathematician. Machine Learning is just mathematics. Mathematics must be correct, hence **all** algorithms are rigoursly unit tested.
* **Documentation:**  I strive for super-thourogh documentation. Admittely at this stage it is hard to keep up. But after release 1.1. I will redocument the code base.
* **Quality:** Thourough commenting, style-cop compliance, and following good software design principles. Mathematicians have been writting bad code for too long. This stops now. Quality is as important as correctness.
* **Approriate Efficeny:** Code is optimized where it **needs** to be optimized. Let's be honest. This is C#. I can't compete with C++, but more and more people are using high-level languages for data-science. When an optimization can drastically improve performance (e.g., using k-d-trees, multi-threading) I optimize. When optimization complicates the code for a 1% speed increase, I don't.


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
* A KD-Tree implementation.
* Updating kNN, kMeans and ENN to use a KD-Tree
