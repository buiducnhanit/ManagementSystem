import { useEffect } from "react";
import useAutoLogout from "./hook/useAutoLogout"
import AppRouter from "./router"
import { startHubConnection } from "./services/signalRService";

function App() {
  useAutoLogout();
  const token = localStorage.getItem("token") || sessionStorage.getItem("token");
  // console.log(token)
  useEffect(() => {
    if (token) {
      startHubConnection();
    }
  }, [token])

  return (
    <>
      <AppRouter />
    </>
  )
}

export default App
