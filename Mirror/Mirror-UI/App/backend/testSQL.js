const {json} = require("express");
const Connection = require('tedious').Connection;
var config = {
    server: 'messages.database.windows.net',  //update me
    authentication: {
        type: 'default',
        options: {
            userName: 'MSGadmin', //update me
            password: 'zubur123!'  //update me
        }
    },
    options: {
        encrypt: true,
        database: 'messages',  //update me
        trustServerCertificate: false,
        rowCollectionOnRequestCompletion: true
    }
};
const getData = (type,user_id) => new Promise( (resolve, reject) =>  {
    var connection = new Connection(config);
    connection.on('connect', function (err) {
        // If no error, then good to proceed.
        console.log("Connected");
        executeStatement(type,user_id).then(ok=> {resolve(ok)})
            .catch(err=> {
                return reject(err)
            });
    });

    connection.connect();

    var Request = require('tedious').Request;
    var query = '';

    const executeStatement = (type, user_id) => new Promise( (resolve, reject) =>  {
        const jsonArray = [];
        if (type==='reminders'){
            query=`select * from trypersongroupid_reminders where userid = '${user_id}';`
        } else if (type==='messages'){
            query=`select * from trypersongroupid_messages where recieverID = '${user_id}';`
        }
        else{
            return 'byyyyeeeeee';
        }
        console.log(query)
        let request = new Request(query, function (err, rowCount, rows) {
            if (err) {
                console.log(err)
                return reject(err)
            }
            rows.forEach(function (columns) {
                let rowObject = {};
                columns.forEach(function (column) {
                    rowObject[column.metadata.colName] = column.value;
                });
                jsonArray.push(rowObject)
            });
            resolve(jsonArray);
        });

        // Close the connection after the final event emitted by the request, after the callback passes
        request.on("requestCompleted", function (rowCount, more) {
            connection.close();
        });
        return connection.execSql(request);

    })
})

module.exports = getData