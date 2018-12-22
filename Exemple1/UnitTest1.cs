using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Exemple1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_create_a_store_in_cloud()
        {
            // This is the main CloudInfrastructure class. You will need to put your code there.
            // The CloudInfrastructure class can also call other classes that you can create if needed.
            CloudInfrastructure cloud = new CloudInfrastructure();

            cloud.CreateStore("myFiles");
            cloud.UploadDocument("myFiles", "book.pdf");

            Assert.AreEqual("myFiles:book.pdf", cloud.ListStores()); // make sure the cloud.listStores() method return the expected output
        }
        /*
        * We can create multiple stores in the cloud, and upload several documents to each one of them.
        */

        [TestMethod]
        public void can_create_multiple_stores_in_cloud()
        {
            CloudInfrastructure cloud = new CloudInfrastructure();

            cloud.CreateStore("myFiles");
            cloud.CreateStore("myImages");
            cloud.UploadDocument("myImages", "picture.jpeg", "profile.png");

            Assert.AreEqual("myFiles:empty||myImages:picture.jpeg, profile.png", cloud.ListStores());
        }


        /*
        * We can also delete or empty a store. When a store does not contain any documents, "empty" is displayed.
        */

        [TestMethod]
        public void Can_delete_and_empty_stores_in_cloud()
        {
            CloudInfrastructure cloud = new CloudInfrastructure();

            cloud.CreateStore("myFiles");
            cloud.CreateStore("myImages");
            cloud.UploadDocument("myImages", "picture.jpeg", "profile.png");

            cloud.DeleteStore("myFiles"); // delete a store
            Assert.AreEqual("myImages:picture.jpeg, profile.png", cloud.ListStores());

            cloud.EmptyStore("myImages"); // empty a store
            Assert.AreEqual("myImages:empty", cloud.ListStores()); // an empty store is display as "empty"
        }

        /*
        * The creation of a store with a name that is already used will throw the StoreException.
        */

        [TestMethod]
        [ExpectedException(typeof(StoreException))]
        public void Cannot_create_stores_with_same_names()
        {
            CloudInfrastructure cloud = new CloudInfrastructure();

            cloud.CreateStore("myFiles");
            cloud.CreateStore("myFiles"); //cannot create stores with same names
        }

        /*
        * We move here to the second part of the test, virtual machines (VM). We can create several VMs
        * in the cloud. Each VM can have three possible statuses : Inactive, Running or Stopped. A new
        * VM is always Inactive at its creation. We can then start or stop it.
        */

        [TestMethod]
        public void Create_machines()
        {
            CloudInfrastructure cloud = new CloudInfrastructure();

            // create a new machine with 4 parameters : name, operating system, disk size, memory.
            cloud.CreateMachine("machine1", "Linux", "50gb", "8gb");
            cloud.CreateMachine("machine2", "Windows", "20gb", "4gb");

            // Remember, all machines are inactive by default.
            Assert.AreEqual("machine1:Inactive||machine2:Inactive", cloud.ListMachines());

            cloud.StartMachine("machine1"); // start the machine "machine1"
            Assert.AreEqual("machine1:running||machine2:Inactive", cloud.ListMachines());

            cloud.StartMachine("machine2");
            cloud.StopMachine("machine1"); // stop machine "machine1"
            Assert.AreEqual("machine1:stopped||machine2:running", cloud.ListMachines());
        }

        /*
        * Trying to start an already running VM will throw a MachineStateException
        */

        [TestMethod]
        [ExpectedException(typeof(StoreException))]
        public void cannot_launch_already_started_machine()
        {
            CloudInfrastructure cloud = new CloudInfrastructure();

            cloud.CreateMachine("machine1", "Linux", "50gb", "8gb");
            cloud.StartMachine("machine1");
            Assert.AreEqual("machine1:running", cloud.ListMachines());

            cloud.StartMachine("machine1"); // will throw the exception
        }

        /**
        * For every VM, we can check the used Disk and memory. The memory is consumed only when a
        * machine is running. The disk size is always used, even if the VM is not running.
        */

        [TestMethod]
        public void Can_check_used_disk_and_ram_per_machine()
        {

            CloudInfrastructure cloud = new CloudInfrastructure();
            cloud.CreateMachine("machine1", "Linux", "50gb", "8gb");
            Assert.AreEqual("machine1:Inactive", cloud.ListMachines());

            Assert.AreEqual(0, cloud.UsedMemoryMachine("machine1"), 0.00001); // Only running machines consume memory
            Assert.AreEqual(50, cloud.UsedDiskMachine("machine1"), 0.00001); // the disk is always consumed

            cloud.StartMachine("machine1");
            Assert.AreEqual(8, cloud.UsedMemoryMachine("machine1"), 0.00001); // All the machine memory is used as it is now running
            Assert.AreEqual(50, cloud.UsedDiskMachine("machine1"), 0.00001);

            cloud.StopMachine("machine1");
            // The memory will be released as the machine has been stopped
            Assert.AreEqual(0, cloud.UsedMemoryMachine("machine1"), 0.00001);
            Assert.AreEqual(50, cloud.UsedDiskMachine("machine1"), 0.00001);
        }

        /*
        * Same as VMs, we can check the used disk in a storage. A storage does not consume any memory,
        * only disk space.
        * 
        * To simplify the exercise, we will suppose that all documents have one size = 100mb = 0.100gb.
        * The disk used by a store is the sum of the sizes of all documents inside.
        */

        [TestMethod]
        public void Can_check_used_disk_per_store()
        {
            CloudInfrastructure cloud = new CloudInfrastructure();

            cloud.CreateStore("myImages");
            cloud.UploadDocument("myImages", "picture.jpeg");

            // we suppose all documents have the same size which is 0.1gb
            Assert.AreEqual(0.100, cloud.UsedDiskStore("myImages"), 0.00001);

            cloud.UploadDocument("myImages", "profile.png");
            // a second document was inserted, used disk in "muImages" = 200mb
            Assert.AreEqual(0.200, cloud.UsedDiskStore("myImages"), 0.00001);

        }

        /**
        * In this test, we can check the used disk and used memory of all machines and stores existing
        * in the cloud.
        */

        [TestMethod]
        public void can_check_aggregated_data_for_all_machines_and_stores()
        {
            CloudInfrastructure cloud = new CloudInfrastructure();

            cloud.CreateMachine("machine1", "Linux", "50gb", "8gb");
            cloud.CreateMachine("machine2", "Windows", "20gb", "4gb");
            Assert.AreEqual("machine1:Inactive||machine2:Inactive", cloud.ListMachines());

            // globalUsedDisk method should return the used disk of all machines and stores existing in the cloud
            // for now 2 machines exists, with 50gb and 20gb disk sizes = 70gb
            Assert.AreEqual(70, cloud.GlobalUsedDisk(), 0.00001);

            // machines are inactive, no memory is used
            Assert.AreEqual(0, cloud.GlobalUsedMemory(), 0.00001);

           cloud.StartMachine("machine1");
            Assert.AreEqual(70, cloud.GlobalUsedDisk(), 0.00001);
            Assert.AreEqual(8, cloud.GlobalUsedMemory(), 0.00001);

            cloud.StartMachine("machine2");
            Assert.AreEqual(70, cloud.GlobalUsedDisk(), 0.00001);
            Assert.AreEqual(12, cloud.GlobalUsedMemory(), 0.00001);

            cloud.CreateStore("myImages");
            cloud.UploadDocument("myImages", "picture.jpeg");

            // 2 machines and 1 documents now exists in the cloud
            Assert.AreEqual(70.100, cloud.GlobalUsedDisk(), 0.00001);
            Assert.AreEqual(12, cloud.GlobalUsedMemory(), 0.00001);

            cloud.UploadDocument("myImages", "profile.png");
            Assert.AreEqual(70.200, cloud.GlobalUsedDisk(), 0.00001);
            Assert.AreEqual(12, cloud.GlobalUsedMemory(), 0.00001);

            cloud.StopMachine("machine1");

            Assert.AreEqual(70.200, cloud.GlobalUsedDisk(), 0.00001);
            Assert.AreEqual(4, cloud.GlobalUsedMemory(), 0.00001);

              cloud.EmptyStore("myImages");
              Assert.AreEqual(70, cloud.GlobalUsedDisk(), 0.00001);
              Assert.AreEqual(4, cloud.GlobalUsedMemory(), 0.00001);
        }

    }
}