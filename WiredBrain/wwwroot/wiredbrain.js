// WebSocket = undefined;
//EventSource = undefined;
//, signalR.HttpTransportType.LongPolling

/**
 * When writing a production app, it is not recommended to write unstructured
 * Javascript like this. It's better to use component-based library such as
 * Angular, React, Vue. This is just for demo purposes which doesn't involved
 * the complexities of using such libraries.
 */
let connection = null;

setupConnection = () => {
    /**
     * We are creating a hub connection through HubConnectionBuilder object. With its
     * nice fluent API, we are giving it the URL of the hub that we should connect
     * to and tell it to .build() the connection. From this call, we get the connection
     * back, which we stored it on the variable. With this variable/object, we can react
     * to the various function calls that are initiated by the server by using the
     * "on" function.
     */
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/coffeehub") // SignalR handles the transport using WebSocket, ServerSentEvents, LongPolling
        // .withUrl("/coffeehub", signalR.HttpTransportType.LongPolling) // specify a transport
        .build();

    /**
     * If there is a status update, the server calls "ReceiveOrderUpdate". The function
     * name is the first parameter of the "on" function. The second parameter is a function
     * that is called when an update comes in. The parameter of that function contains
     * the update.
     */
    connection.on("ReceiveOrderUpdate", (update) => {
            const statusDiv = document.getElementById("status");
            statusDiv.innerHTML = update;
        }
    );

    /**
     * For "NewOrder", the return function parameter is a complex object. Since
     * de-serialization is handled automatically, we can just access the properties of the
     * object. Noticed that the property name is converted to camel-case.
     */
    connection.on("NewOrder", function(order) {
            var statusDiv = document.getElementById("status");
            statusDiv.innerHTML = "Someone ordered an " + order.product;
        }
    );

    /**
     * When it received the "finished" call, the client closes the connection.
     */
    connection.on("Finished", function() {
            connection.stop();
        }
    );

    /**
     * When we are done with the implementation of all "on" function calls that we
     * want to react to, we open the connection. It's a good idea to first create the handles
     * of the various function calls, and then open the connection, so you are sure you
     * are not missing any messages.
     */
    connection.start()
        .catch(err => console.error(err.toString())); 
};

/**
 * Setting up the connection is executed right away when the user opens the page.
 */
setupConnection();

document.getElementById("submit").addEventListener("click", e => {
    e.preventDefault();
    const product = document.getElementById("product").value;
    const size = document.getElementById("size").value;

    /**
     * Fetch returns a promise. Promises are one of the ways to do asynchronous
     * programming in Javascript. It is an object that results, when the result of,
     * in this case, the HTTP request is known. When the result is known, we can
     * then perform actions.
     */
    fetch("/Coffee",
        {
            method: "POST",
            body: JSON.stringify({ product, size }),
            headers: {
               'content-type': 'application/json'
            }
        })
        /**
         * We are getting the returned order ID by getting the text that was sent
         * back and then we are updating the HTML page with that ID
         */
        .then(response => response.text())
        .then(id => connection.invoke("GetUpdateForOrder", id));
});