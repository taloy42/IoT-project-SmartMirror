import './App.css';
import './style.css';
import React, { useEffect, useState } from "react";
import Weather from "./components/weather/weather"
import Messages from "./components/messages/messages";
import Reminders from "./components/reminders/reminders";
import HelloPerson from "./components/helloPerson/helloPerson";
import {Transition} from "react-transition-group";
import backround from "./images/Firefly-33.3s-1084px.svg";
import State from './components/state/state'
const duration = 1000;
const nobody_id = '00000000-0000-0000-0000-000000000000'
const defaultStyle = {
  transition: `opacity ${duration}ms ease-in-out`,
  opacity: 0,
};
const transitionStyles = {
  entering: { opacity: 0 },
  entered: { opacity: 1 },
  exiting: { opacity: 1 },
  exited: { opacity: 0 },
};

function App() {
  const REACT_APP_API_URL = 'https://api.openweathermap.org/data/2.5';
  const REACT_APP_API_KEY = '8990249cacc2f1533d19c3df56dd98ce';
  const [lat, setLat] = useState([32.1067]);
  const [long, setLong] = useState([34.8037]);
  const [data, setData] = useState([]);
  const [messages, setMessages] = useState([]);
  const [reminders, setReminders] = useState([]);
  const [state, setState]= useState('');
  const [userID,setUserID] = useState('00000000-0000-0000-0000-000000000000');
  const [userName, setUserName] =useState('')

  useEffect(() => {
    const ws = new WebSocket("ws://127.0.0.1:8000");

    const apiCall = {
      event: "",
      data: { channel: "hello from client!" },
    };

    ws.onopen = (event) => {
      console.log('opened')
      ws.send(JSON.stringify(apiCall));
    };

    ws.onmessage = function (event) {
      const object = JSON.parse(event.data);
      const data = object.data;
      const name = object.name;
      try {
        switch(data) {
          case 'unrecognized':
            setUserID(nobody_id)
            setState('notInSystem');
            setTimeout(()=> {
              setState('')
            }, 2000)

            break;
          case 'face_detected':
            setState('tryingToMatch');
            break;
          default:
            if(data==='0'){
              setUserID(nobody_id)
              setState('')
            }
            else{
              setState('Matched')
              setTimeout(()=> {
                setUserID(data)
              }, 3500)
              setUserName(name)

            }

        }
      } catch (err) {
        console.log(err);
      }
    };
  }, []);


  useEffect(() => {
    const fetchWeatherData = async() => {
      navigator.geolocation.getCurrentPosition(function (position) {
        setLat(position.coords.latitude);
        setLong(position.coords.longitude);
      });

        await fetch(`${REACT_APP_API_URL}/weather/?lat=${lat}&lon=${long}&units=metric&APPID=${REACT_APP_API_KEY}`)
          .then(res => res.json())
          .then(result => {
            setData(result)
          });
    }
      fetchWeatherData();
  }, [lat, long]);

  useEffect(() => {
    const fetchMessagesData = async() => {
      navigator.geolocation.getCurrentPosition(function (position) {
        setLat(position.coords.latitude);
        setLong(position.coords.longitude);
      });



        await fetch(`http://127.0.0.1:3001/show_messages?user_id=${userID}`)
          .then(res => res.json())
          .then(result => {
            setMessages(result)
          });
    }
      fetchMessagesData();
  }, [userID]);

  useEffect(() => {
    const fetchRemindersData = async() => {
      await fetch(`http://127.0.0.1:3001/show_reminders?user_id=${userID}`)
          .then(res => res.json())
          .then(result => {
            setReminders(result)
          });
    }
    fetchRemindersData();
  }, [userID]);


  return (
      <div style={{backgroundImage: `url(${backround})`, height: '1080px'}}>
        {(userID===nobody_id) ? ( <div className='state'>{State(state)}</div>) :
            (<Transition in={userID!=='0'} timeout={300}>
          {(state) => ( <div style={{...defaultStyle, ...transitionStyles[state]}}>
            <div className="position-absolute top-500 start-400" >
              <div className='mirror'>
                <div style={{width:'20%'}}>
                  <HelloPerson person={userName} userID={userID}/>
                  <div className="App">
                    {(typeof data.main != 'undefined') ? (
                        <Weather weatherData={data}/>): (<div className='weather col-sm-7'>Loading weather...</div>)}
                  </div>
                </div>

                <div style={{width: '30%'}}>
                  {/*<CSSTransition in={detectedUser} timeout={300} classNames="App" mountOnEnter>*/}
                    <div className="App">
                      {(typeof messages.rows != 'undefined') ? (
                          <Messages messagesData={messages.rows} />): (<div className='weather col-sm-7'> Loading Messages...</div>)}
                    </div>
                  {/*</CSSTransition>*/}
                  <div className="App">
                    {(typeof reminders.rows != 'undefined') ? (
                        <Reminders reminderData={reminders.rows}/>): (<div className='weather col-sm-7'> Loading Reminders...</div>)}
                  </div>
                </div>
              </div>
            </div>
          </div>)}
        </Transition>)}
      </div>

  );
}

export default App;
