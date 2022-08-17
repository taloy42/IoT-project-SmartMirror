const express = require("express");
const getData = require("./testSQL.js");
const cors = require("cors");
const PORT = process.env.PORT || 3001;

const app = express();
app.use(cors())
app.get("/show_messages", (req, res) => {
    const user_id = req.query.user_id;
    getData('messages',user_id).then(result=>{return res.json({rows:result})}).catch(err=>{console.log(err); return {}})

});

app.get("/show_reminders", (req, res) => {
    const user_id = req.query.user_id;
    getData('reminders',user_id).then(result=>{return res.json({rows:result})}).catch(err=>{console.log(err); return {}})

});

app.get("/api", (req, res) => {
    res.json({ message: "Hello from server!" });
});

app.listen(PORT, () => {
    console.log(`Server listening on ${PORT}`);
});