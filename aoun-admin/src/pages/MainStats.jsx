import { useState, useEffect } from 'react';
import axios from 'axios';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { Activity, Users, HeartHandshake, Building } from 'lucide-react';

export default function MainStats() {
    const [stats, setStats] = useState(null);

    useEffect(() => {
        const fetchStats = async () => {
            try {
                const token = localStorage.getItem('admin_token');
                const res = await axios.get('http://localhost:5055/api/AdminDashboard/stats', {
                    headers: { Authorization: `Bearer ${token}` }
                });
                setStats(res.data.data);
            } catch (err) { console.error("Error fetching stats", err); }
        };
        fetchStats();
    }, []);

    if (!stats) return <div className="text-center mt-20 text-xl font-bold">جاري تحميل البيانات...</div>;

    return (
        <div className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
                <div className="bg-white p-6 rounded-xl shadow-sm border-r-4 border-blue-500 flex justify-between items-center">
                    <div><p className="text-gray-500 text-sm mb-1">إجمالي التبرعات</p><h3 className="text-2xl font-bold">{stats.totalDonations} ج.م</h3></div>
                    <HeartHandshake className="text-blue-500 w-10 h-10" />
                </div>
                <div className="bg-white p-6 rounded-xl shadow-sm border-r-4 border-teal-500 flex justify-between items-center">
                    <div><p className="text-gray-500 text-sm mb-1">المستخدمين</p><h3 className="text-2xl font-bold">{stats.totalUsers}</h3></div>
                    <Users className="text-teal-500 w-10 h-10" />
                </div>
                <div className="bg-white p-6 rounded-xl shadow-sm border-r-4 border-orange-500 flex justify-between items-center">
                    <div><p className="text-gray-500 text-sm mb-1">الحالات النشطة</p><h3 className="text-2xl font-bold">{stats.activeCases}</h3></div>
                    <Activity className="text-orange-500 w-10 h-10" />
                </div>
                <div className="bg-white p-6 rounded-xl shadow-sm border-r-4 border-purple-500 flex justify-between items-center">
                    <div><p className="text-gray-500 text-sm mb-1">الجمعيات</p><h3 className="text-2xl font-bold">{stats.totalCharities}</h3></div>
                    <Building className="text-purple-500 w-10 h-10" />
                </div>
            </div>
            <div className="bg-white p-6 rounded-xl shadow-sm">
                <h3 className="text-lg font-bold mb-4 border-b pb-2">مؤشر التبرعات الأخير</h3>
                <div className="h-72">
                    <ResponsiveContainer width="100%" height="100%">
                        <LineChart data={stats.recentDonations}>
                            <CartesianGrid strokeDasharray="3 3" vertical={false} />
                            <XAxis dataKey="date" />
                            <YAxis />
                            <Tooltip />
                            <Line type="monotone" dataKey="amount" stroke="#0d9488" strokeWidth={3} dot={{ r: 6 }} />
                        </LineChart>
                    </ResponsiveContainer>
                </div>
            </div>
        </div>
    );
}