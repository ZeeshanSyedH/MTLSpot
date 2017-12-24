var mongoose = require('mongoose');

var dbURI = 'mongodb://localhost/MTLSpot';

if(process.env.NODE_ENV === 'production')
    {
        dbURI = process.env.MONGOLAB_URI;
    }

mongoose.connect(dbURI);

var gracefulShutdown = function (msg, callback)
{
    mongoose.connection.close(function(){
        console.log('Mongoose disconnected through ' + msg);
        callback();
    });
};

// DATABASE LISTENER METHODS
// UPON DATABASE CONNECTION 
mongoose.connection.on('connected', function(){
    console.log('Mongoose connected to ' + dbURI);
});
// UPON DATABASE ERROR
mongoose.connection.on('error', function(err){
    console.log('Mongoose connection error: ' + err);
});
// UPON DATABASE DISCONNECTION
mongoose.connection.on('disconnected', function(){
    console.log('Mongoose Disconnected');
});
// UPON NODE RESTART
process.on('SIGUSR2', function(){
    gracefulShutdown('nodemon restart', function(){
        process.kill(process.pid,'SIGUSR2')
    })
});
// UPON APP TERMINATION
process.on('SIGINT', function(){
    gracefulShutdown('SIGINT', function(){
        process.exit(0);
    });
});
// UPON HEROKU APP TERMINATION
process.on('SIGTERM', function(){
    gracefulShutdown('Heroku App Shutdown', function(){
        process.exit(0);
    });
});

require('./locations');