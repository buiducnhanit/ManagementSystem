import useAutoLogout from "./hook/useAutoLogout"
import AppRouter from "./router"

function App() {
  useAutoLogout();
  return (
    <>
      <AppRouter />
    </>
  )
}

export default App
