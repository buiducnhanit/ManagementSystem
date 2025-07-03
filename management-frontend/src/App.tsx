import { useEffect } from "react";
import useAutoLogout from "./hook/useAutoLogout"
import AppRouter from "./router"
import { startHubConnection } from "./services/signalRService";

function App() {
  useAutoLogout();

  useEffect(() => {
    startHubConnection();
  }, [])

  return (
    <>
      <AppRouter />
    </>
  )
}

export default App
