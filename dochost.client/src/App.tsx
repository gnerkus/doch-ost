import './App.css';
import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import Dashboard from "./dashboard/Dashboard.tsx";
import AuthProvider from "./auth/AuthContext.tsx";

const queryClient = new QueryClient();

function App() {

    return (
        <AuthProvider>
            <QueryClientProvider client={queryClient}>
                <Dashboard />
            </QueryClientProvider>
        </AuthProvider>
    )
}

export default App;