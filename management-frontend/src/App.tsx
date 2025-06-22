import useAutoLogout from "./hook/useAutoLogout"
// import useIdleSessionTimeout from "./hook/useIdleSessionTimeout";
import AppRouter from "./router"

function App() {
  useAutoLogout();
  // useIdleSessionTimeout();
  return (
    <>
      <AppRouter />
    </>
  )
}

export default App
